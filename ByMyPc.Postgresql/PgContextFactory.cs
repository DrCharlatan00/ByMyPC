using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace ByMyPc.Postgresql
{
    public class PgContextFactory : IDesignTimeDbContextFactory<PgContext>
    {
        public PgContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
            optionsBuilder.UseNpgsql("Password=ByMyPcPGPass;Persist Security Info=True;Username=drcharlatan;Database=bymypcdb;Host=localhost");
            return new PgContext(optionsBuilder.Options);
        }
    }
}
