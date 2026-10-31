using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace ByMyPc.Postgresql
{
    public class PgContextFactory : IDesignTimeDbContextFactory<PgContext>
    {
        public PgContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
            try
            {

                EnvReader.Load(".env");
                string? key = Environment.GetEnvironmentVariable("confdb");
                if (key is not null)
                {
                    optionsBuilder.UseNpgsql(key);
                    return new PgContext(optionsBuilder.Options);
                }
            }
            catch (Exception)
            {
            }
            optionsBuilder.UseNpgsql("Password=ByMyPcPGPass;Persist Security Info=True;Username=drcharlatan;Database=bymypcdb;Host=localhost:5432");
            return new PgContext(optionsBuilder.Options);
        }
    }
}
