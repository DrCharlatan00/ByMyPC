using ByMyPc.Postgresql;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http.Connections;
using System.Security.Cryptography;

namespace ByMyPC.Middlewares
{
    public class MiddlewareExceptions(RequestDelegate requestDelegate, ILogger<MiddlewareExceptions> logger, PgContext context)
    {
        private readonly RequestDelegate requestDelegate = requestDelegate;
        private readonly ILogger<MiddlewareExceptions> logger = logger;
        private readonly PgContext context = context;

        public async Task InvokeAsync(HttpContext httpContext) {
            string messageUser = string.Empty;
            int StatusCode = StatusCodes.Status500InternalServerError;
            try
            {
                await requestDelegate(httpContext);
            }
            catch (Exception ex) when (ex is OperationsException<object> ecx) {
                try
                {
                    var problemDetail = new ProblemDetailModel
                    {
                        ID = Guid.NewGuid(),
                        NameException = ex.GetType().FullName ?? "Unknown",
                        NameAPIOrServer = "API ByMyPc 4",
                        TimeProblemCreated = DateTimeOffset.UtcNow,
                        Details = ex.Message + "\n" + $"Problem created with collection {ecx.NameCollection}, API Set bad request ? = {ecx.BadRq}"
                    };
                    await context.Problems.AddAsync(problemDetail);
                }
                catch (Exception exProb)
                {
                    logger.LogError(exProb, "Can't create to db message");
                }


                logger.LogError(ex,"Operations aborted {exmess}\n SystemFall?:{bq}", ex.Message,ecx.BadRq);
                messageUser = ecx.BadRq ? "Operation cancelled. The database rejected your data. " : "Server unexpect abord operation, Please contact to Administrator";
                StatusCode = ecx.BadRq ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex) when (ex is IOperationException exs)
            {

                logger.LogError(ex, "Middleware catch exception \nCollecion = {collname} \nTypeCollection = {@type}", exs.NameCollection, exs.CollectionThrow);
                messageUser = ex switch
                {
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
                try
                {
                    var problemDetail = new ProblemDetailModel
                    {
                        ID = Guid.NewGuid(),
                        NameException = ex.GetType().FullName ?? "Unknown",
                        NameAPIOrServer = "API ByMyPc 4",
                        TimeProblemCreated = DateTimeOffset.UtcNow,
                        Details = ex.Message + "\n" + $"Problem created with collection {exs.NameCollection}, API Set Code = {StatusCode}"
                    };
                    await context.Problems.AddAsync(problemDetail);
                }
                catch (Exception exProb)
                {
                    logger.LogError(exProb, "Can't create to db message");
                }


                httpContext.Response.StatusCode = StatusCode;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = messageUser
                });
                return;
            }
            catch (Exception ex) when (ex is ValidationException validationException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = "Validator fail",
                    ValidatorError = validationException.Errors
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Middleware catch exception {@ex}", ex);
                messageUser = ex switch
                {
                    ArgumentNullException => "Invalid parameter received, parameter is null",
                    ArgumentException => "Invalid parameter received",
                    NullReferenceException => "The server was unable to process the parameter; this may be an internal error or an empty parameter was passed.",
                    IOperationException => "The database is temporarily unavailable due to internal reasons.",
                    _ => "Unknown error in server, please call administrator"
                };
                StatusCode = ex switch
                {
                    ArgumentNullException => StatusCodes.Status400BadRequest,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    NullReferenceException => StatusCodes.Status500InternalServerError,
                    IOperationException => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status500InternalServerError
                };
                try
                {
                    var problemDetail = new ProblemDetailModel
                    {
                        ID = Guid.NewGuid(),
                        NameException = ex.GetType().FullName ?? "Unknown",
                        NameAPIOrServer = "API ByMyPc 4",
                        TimeProblemCreated = DateTimeOffset.UtcNow,
                        Details = $"Middleware catch exception, name = {ex.GetType().FullName ?? "unknown"} ex message = {ex.Message}"
                    };
                    await context.Problems.AddAsync(problemDetail);
                }
                catch (Exception exProb)
                {
                    logger.LogError(exProb, "Can't create to db message");
                }

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
