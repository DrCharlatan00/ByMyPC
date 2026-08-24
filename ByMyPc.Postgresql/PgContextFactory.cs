using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace ByMyPc.Postgresql
{
    public class PgContextFactory : IDesignTimeDbContextFactory<PgContext>
    {
        public PgContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
            optionsBuilder.UseNpgsql("Password=1303;Persist Security Info=True;Username=DrCharlatan;Database=ByMyPc;Host=localhost");
            return new PgContext(optionsBuilder.Options);
        }
    }
}
