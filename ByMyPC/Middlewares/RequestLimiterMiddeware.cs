
namespace ByMyPC.Middlewares
{
    public class RequestLimiterMiddeware(RequestDelegate requestDelegate)
    {
        private const string message = "To many request, try again later";
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(50, 50);
        private readonly RequestDelegate requestDelegate = requestDelegate;

        public async Task InvokeAsync(HttpContext content)
        {
            var entered = await semaphore.WaitAsync(
                TimeSpan.FromSeconds(5),
                content.RequestAborted
                );

            if (!entered) {
                content.Response.StatusCode = StatusCodes.Status409Conflict;

                content.Response.WriteAsJsonAsync(new  {
                    Message = message
                });
                return;   
            }
            try
            {
                await requestDelegate(content);
            }
            finally { semaphore.Release(); }
        }
    }
}
