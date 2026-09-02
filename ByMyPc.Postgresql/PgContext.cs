using ByMyPc.Postgresql.Models;
using Microsoft.EntityFrameworkCore;

namespace ByMyPc.Postgresql
{
    public class PgContext : DbContext
    {
        public DbSet<PcDbModel> PCs { get; set; }

        public DbSet<CpuDbModel> CPUs { get; set; }

        public DbSet<GpuDbModel> GPUs { get; set; }

        public DbSet<MotherboardDbModel> Motherboards { get; set; }

        public DbSet<RamDbModel> RAMs { get; set; }

        public DbSet<HDDDbModel> HDDs { get; set; }

        public DbSet<PcRamDbModel> PcRams { get; set; }

        public DbSet<PcHddDbModel> PcHdds { get; set; }

        public PgContext(DbContextOptions<PgContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //cpu 
            modelBuilder.Entity<PcDbModel>()
                .HasOne(x => x.Cpu)
                .WithMany()
                .HasForeignKey(x => x.CpuId)
                .OnDelete(DeleteBehavior.SetNull);

            //motherboard
            modelBuilder.Entity<PcDbModel>()
                .HasOne(x => x.Motherboard)
                .WithMany()
                .HasForeignKey(x => x.MotherboardId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MotherboardDbModel>().HasIndex(x => x.Name).HasMethod("gin").HasOperators("gin_trgm_ops");

            //GPU
            modelBuilder.Entity<PcDbModel>()
                .HasOne(x => x.Gpu)
                .WithMany()
                .HasForeignKey(x => x.GpuId)
                .OnDelete(DeleteBehavior.SetNull);

            //Ram
            modelBuilder.Entity<PcRamDbModel>()
               .HasKey(x => new
               {
                   x.PcId,
                   x.RamId,
                   x.Slot
               });

            modelBuilder.Entity<PcRamDbModel>()
                .HasOne(x => x.Pc)
                .WithMany(x => x.Rams)
                .HasForeignKey(x => x.PcId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PcRamDbModel>()
                .HasOne(x => x.Ram)
                .WithMany()
                .HasForeignKey(x => x.RamId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PcHddDbModel>()
            .HasKey(x => new
                {
                 x.PcId,
                x.HddId
                });


            modelBuilder.Entity<PcHddDbModel>()
                .HasOne(x => x.Pc)
                .WithMany(x => x.HDDs)
                .HasForeignKey(x => x.PcId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PcHddDbModel>()
                .HasOne(x => x.Hdd)
                .WithMany()
                .HasForeignKey(x => x.HddId)
                .OnDelete(DeleteBehavior.Restrict);


            //PSU
            modelBuilder.Entity<PcDbModel>()
                .HasOne(x => x.PSU)
                .WithMany()
                .HasForeignKey(x => x.PSUId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
