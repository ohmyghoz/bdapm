using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SyncWebService.DataModel
{
    public partial class DataEntities : DbContext
    {
        public DataEntities()
        {
        }

        public DataEntities(DbContextOptions<DataEntities> options)
            : base(options)
        {
        }

        public virtual DbSet<HiveSync> HiveSync { get; set; }
        public virtual DbSet<LogEtl> LogEtls { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Settings1.Default.ConnStringSQL);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HiveSync>(entity =>
            {
                entity.HasKey(e => e.sync_id);

                entity.Property(e => e.pperiode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.pprocess)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.sync_status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LogEtl>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("Log_ETL");

                entity.Property(e => e.LogId).HasColumnName("log_id");
                entity.Property(e => e.LogDate)
                    .HasPrecision(0)
                    .HasColumnName("log_date");
                entity.Property(e => e.LogDeleteCnt).HasColumnName("log_delete_cnt");
                entity.Property(e => e.LogEnd)
                    .HasColumnType("datetime")
                    .HasColumnName("log_end");
                entity.Property(e => e.LogErrmessage)
                    .IsUnicode(false)
                    .HasColumnName("log_errmessage");
                entity.Property(e => e.LogInsertCnt).HasColumnName("log_insert_cnt");
                entity.Property(e => e.LogPeriode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("log_periode");
                entity.Property(e => e.LogStart)
                    .HasColumnType("datetime")
                    .HasColumnName("log_start");
                entity.Property(e => e.LogStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("log_status");
                entity.Property(e => e.LogTipe)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("log_tipe");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
