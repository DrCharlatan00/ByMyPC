using ByMyPc.Postgresql;
using Microsoft.EntityFrameworkCore;

namespace ByMyPC.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task InitializeDatebaseAsync(
                this WebApplication app
            )
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<PgContext>();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PgContext>>();

            try
            {
                bool connected = false;
                for (int i = 0; i < 5; i++)
                {
                    connected = await db.Database.CanConnectAsync();

                    if (connected) break;

                    logger.LogWarning("connect is failed, count try {count}",i);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
                if (!connected) {
                    logger.LogError("Do not connect to db!");

                    return;
                }

                await db.Database.ExecuteSqlRawAsync(
                    "CREATE EXTENSION IF NOT EXISTS pg_trgm;"); 

                var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToArray();

                if (pendingMigrations.Length == 0) 
                {
                    logger.LogInformation("Migrations all, skip....");
                    return;
                }

                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Migration fail, app throw");
                throw;
            }
        }
    }
}
