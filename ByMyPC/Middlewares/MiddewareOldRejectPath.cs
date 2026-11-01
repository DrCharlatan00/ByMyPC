namespace ByMyPC.Middlewares
{
    public class MiddewareOldRejectPath(RequestDelegate requestDelegate, ILogger<MiddewareOldRejectPath> logger)
    {
        private readonly RequestDelegate requestDelegate = requestDelegate;
        private readonly ILogger<MiddewareOldRejectPath> logger = logger;

        public List<string> WrongPath = new List<string> {
            "api/gpu/create",
            "api/cpu/create",
            "api/motherboard/create",
            "api/psu/create",
            "api/ram/create",
            "api/pc/create"
        };

        public List<string> LegacyPath = new List<string> {
                 "api/gpu/create/v1",

        };

        public async Task InvokeAsync(HttpContext content) {
            foreach (var item in WrongPath)
            {
                if (content.Request.Path == item) {
                    content.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await content.Response.WriteAsJsonAsync(new
                    {
                        Message = "Use version function"
                    });
                    return;
                }
            }
            foreach (var item in LegacyPath)
            {
                if (content.Request.Path == item)
                {
                    content.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await content.Response.WriteAsJsonAsync(new
                    {
                        Message = "This Version is legacy and now work now"
                    });
                    return;
                }
            }

        }
    }
}
