namespace ByMyPC.Middlewares
{
    public class MiddewareOldRejectPath(RequestDelegate requestDelegate, ILogger<MiddewareOldRejectPath> logger)
    {
        private readonly RequestDelegate requestDelegate = requestDelegate;
        private readonly ILogger<MiddewareOldRejectPath> logger = logger;

        public List<string> WrongPath = new List<string> {
            "/api/gpu/create",
            "/api/cpu/create",
            "/api/motherboard/create",
            "/api/psu/create",
            "/api/ram/create",
            "/api/pc/create",
            "/api/gpu/update",
            "/api/cpu/update",
            "/api/motherboard/update",
            "/api/psu/update",
            "/api/ram/update",
            "/api/pc/update",
            "/api/gpu/remove",
            "/api/cpu/remove",
            "/api/motherboard/remove",
            "/api/psu/remove",
            "/api/ram/remove",
            "/api/pc/remove"
        };

        public List<string> LegacyPath = new List<string> {
                 "/api/gpu/create/v1",
                 "/api/gpu/update/v1",
                 "/api/cpu/create/v1",
                 "/api/cpu/update/v1",
                 "/api/hdd/create/v1",
                 "/api/hdd/update/v1",
                 "/api/motherboard/create/v1",
                 "/api/motherboard/update/v1",
                 "/api/psu/create/v1",
                 "/api/psu/update/v1",
                 "/api/pc/create/v1",
                 "/api/pc/update/v1",
                 "/api/ram/create/v1",
                 "/api/ram/update/v1",
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

            await requestDelegate(content);
        }
    }
}
