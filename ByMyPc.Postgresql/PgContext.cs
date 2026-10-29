using ByMyPc.Postgresql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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
        public DbSet<PSUDbModel> Psu { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<ProblemDetailModel> Problems { get; set; }
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


            modelBuilder.Entity<CpuDbModel>().HasIndex(x => x.Name).HasMethod("gin").HasOperators("gin_trgm_ops");

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



            //kafka
            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Topic)
                .IsRequired();

                entity.Property(x => x.EventType)
                .IsRequired().HasMaxLength(100);

                entity.Property(x => x.AggregateId)
                  .IsRequired().HasMaxLength(100);

                entity.Property(x => x.Payload)
                .IsRequired()
                .HasColumnType("json");
                ;

                entity.Property(x => x.CreatedAt)
                .IsRequired();

                entity.Property(x => x.ProcessedAt)
                .IsRequired(false);

                entity.Property(x => x.RetryCount)
                .IsRequired().HasDefaultValue(0);


                entity.HasIndex(x => x.CreatedAt);


            });
            modelBuilder.Entity<ProblemDetailModel>(x => {
                x.HasKey(x => x.ID);

                x.Property(x => x.Details).IsRequired();

                x.Property(x => x.NameException).IsRequired().HasMaxLength(100);

                x.Property(x => x.NameAPIOrServer).IsRequired().HasMaxLength(100);

                x.HasIndex(x => x.TimeProblemCreated);

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
