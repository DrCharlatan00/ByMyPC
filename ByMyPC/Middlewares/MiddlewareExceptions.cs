using ByMyPc.Postgresql.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http.Connections;
using System.Security.Cryptography;

namespace ByMyPC.Middlewares
{
    public class MiddlewareExceptions(RequestDelegate requestDelegate, ILogger<MiddlewareExceptions> logger)
    {
        private readonly RequestDelegate requestDelegate = requestDelegate;
        private readonly ILogger<MiddlewareExceptions> logger = logger;

        public async Task InvokeAsync(HttpContext httpContext) {
            string messageUser = string.Empty;
            int StatusCode = StatusCodes.Status500InternalServerError;
            try
            {
                await requestDelegate(httpContext);
            }
            catch (Exception ex) when (ex is IOperationException exs)
            {

                logger.LogError(ex, "Middleware catch exception \nCollecion = {collname} \nTypeCollection = {@type}", exs.NameCollection, exs.CollectionThrow);
                messageUser = ex switch {
                    CreateOperationException<object> => "Create operation is failed",
                    RemoveOperationException<object> => "Remove operation is failed",
                    UpdateOperationException<object> => "Update operation is failed",
                    _ => "Unknow Operation in db is failed"
                };

                StatusCode = ex switch
                {
                    CreateOperationException<object> => StatusCodes.Status409Conflict,
                    RemoveOperationException<object> => StatusCodes.Status409Conflict,
                    UpdateOperationException<object> => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                };
                httpContext.Response.StatusCode = StatusCode;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = messageUser
                });
                return;
            }
            catch (Exception ex) when (ex is ValidationException validationException) {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = "Validator fail",
                    ValidatorError = validationException.Errors
                });
            }
            catch (Exception ex) {
                logger.LogError(ex, "Middleware catch exception {@ex}",ex);
                messageUser = ex switch
                {
                    ArgumentNullException => "Invalid parameter received, parameter is null",
                    ArgumentException => "Invalid parameter received",
                    NullReferenceException => "The server was unable to process the parameter; this may be an internal error or an empty parameter was passed.",
                    IOperationException => "The database is temporarily unavailable due to internal reasons.",
                    _ => "Unknown error in server, please call administrator"
                };
                StatusCode = ex switch {
                    ArgumentNullException => StatusCodes.Status400BadRequest,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    NullReferenceException => StatusCodes.Status500InternalServerError,
                    IOperationException => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status500InternalServerError
                };
                httpContext.Response.StatusCode = StatusCode;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = messageUser
                });
                return;
            }

        }
    }
}
