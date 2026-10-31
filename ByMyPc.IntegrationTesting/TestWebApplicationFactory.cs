using ByMyPc.Postgresql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;


namespace ByMyPc.IntegrationTesting
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(src => {
                var old = src.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PgContext>));

                if (old is not null) src.Remove(old);
                src.AddDbContext<PgContext>(opt => { opt.UseNpgsql("Password=ByMyPcPGPass;Persist Security Info=True;Username=drcharlatan;Database=bymypcdb;Host=localhost:5455"); });

            });
        }
    }
}
