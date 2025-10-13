using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Vodo.Models;

namespace Vodo.DAL.Context
{
    public class VodoContext : DbContext
    {
        public VodoContext(DbContextOptions<VodoContext> options) : base(options) { }
         
        public DbSet<Contractor> Contractors { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobObject> JobObjects { get; set; }

        public DbSet<JobMedia> JobMedias { get; set; }

        public DbSet<JobComment> JobComments { get; set; }

        public DbSet<JobStream> JobStreams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contractor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Автогенерация ID
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Inn).HasMaxLength(12);
                entity.Property(e => e.Payload).HasColumnType("jsonb");
            });

            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Автогенерация ID
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Entity).IsRequired().HasMaxLength(50);
                entity.Property(e => e.User).HasMaxLength(100);
                entity.Property(e => e.Payload).HasColumnType("jsonb");
                entity.Property(e => e.Timestamp).HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<JobObject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Автогенерация ID
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.OwnerDivision).HasMaxLength(100);

                // Owned type Address
                entity.OwnsOne(e => e.Address, address =>
                {
                    address.Property(a => a.Line1).HasMaxLength(200);
                    address.Property(a => a.Line2).HasMaxLength(200);
                    address.Property(a => a.City).HasMaxLength(100);
                    address.Property(a => a.District).HasMaxLength(100);
                    address.Property(a => a.PostalCode).HasMaxLength(20);
                });

                // Geometry (Point) configuration for PostgreSQL
                entity.Property(e => e.Location).HasColumnType("geometry (point)");

                // RowVersion for concurrency
                //entity.Property(e => e.RowVersion).IsRowVersion();
                entity.Property(e => e.RowVersion)
                    .IsConcurrencyToken() // Отмечаем как токен конкурентности
                    .ValueGeneratedOnAddOrUpdate(); // Генерируется при добавлении/обновлении
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Автогенерация ID
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                // Relationships
                entity.HasOne(e => e.JobObject)
                      .WithMany(jo => jo.Jobs)
                      .HasForeignKey(e => e.SiteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Contractor)
                      .WithMany()
                      .HasForeignKey(e => e.ContractorId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Owned type PlannedActualPeriod
                entity.OwnsOne(e => e.PlannedFact, planned =>
                {
                    planned.Property(p => p.PlannedStart);
                    planned.Property(p => p.PlannedEnd);
                    planned.Property(p => p.FactStart);
                    planned.Property(p => p.FactEnd);
                });

                // Enum conversions
                entity.Property(e => e.Type)
                      .HasConversion<string>();

                entity.Property(e => e.Priority)
                      .HasConversion<string>();

                // Geometry configuration for PostgreSQL
                entity.Property(e => e.Geometry).HasColumnType("geometry");

                // RowVersion for concurrency
                //entity.Property(e => e.RowVersion).IsRowVersion();
                entity.Property(e => e.RowVersion)
                    .IsConcurrencyToken() // Отмечаем как токен конкурентности
                    .ValueGeneratedOnAddOrUpdate(); // Генерируется при добавлении/обновлении

                entity.Property(e => e.Status)
                .HasConversion<string>();
            });

            modelBuilder.Entity<JobMedia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Автогенерация ID
                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Kind).HasConversion<string>();
                entity.Property(e => e.Meta).HasColumnType("jsonb");

                entity.HasOne(e => e.Job)
                      .WithMany(j => j.Media)
                      .HasForeignKey(e => e.JobId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<JobComment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Автогенерация ID
                entity.Property(e => e.Text).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Author).HasMaxLength(100);

                entity.HasOne(e => e.Job)
                      .WithMany(j => j.Comments)
                      .HasForeignKey(e => e.JobId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<JobStream>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Автогенерация ID
                entity.Property(e => e.SourceUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PublicUrl).HasMaxLength(500);
                entity.Property(e => e.Kind).HasConversion<string>();

                // Relationships
                entity.HasOne(e => e.JobObject)
                      .WithMany(jo => jo.Streams)
                      .HasForeignKey(e => e.JobObjectId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Job)
                      .WithMany(j => j.Streams)
                      .HasForeignKey(e => e.JobId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Indexes for better performance
            modelBuilder.Entity<EventLog>()
                .HasIndex(e => new { e.Timestamp, e.Entity, e.EntityId });

            modelBuilder.Entity<Job>()
                .HasIndex(e => e.StatusId);

            modelBuilder.Entity<Job>()
                .HasIndex(e => e.ContractorId);

            modelBuilder.Entity<Job>()
                .HasIndex(e => e.SiteId);

            modelBuilder.Entity<JobMedia>()
                .HasIndex(e => e.JobId);

            modelBuilder.Entity<JobComment>()
                .HasIndex(e => e.JobId);

            modelBuilder.Entity<JobStream>()
                .HasIndex(e => new { e.JobId, e.IsActive });

            modelBuilder.Entity<JobStream>()
                .HasIndex(e => new { e.JobObjectId, e.IsActive });
        }

        // Переопределяем SaveChanges для ручного управления версиями
        public override int SaveChanges()
        {
            UpdateRowVersions();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateRowVersions();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateRowVersions()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                // Для сущностей с RowVersion обновляем версию
                var rowVersionProperty = entry.Metadata.FindProperty("RowVersion");
                if (rowVersionProperty != null && rowVersionProperty.ClrType == typeof(byte[]))
                {
                    // Генерируем новый "timestamp" - просто случайные байты
                    // В реальном приложении лучше использовать более надежный метод
                    var random = new Random();
                    var newRowVersion = new byte[8];
                    random.NextBytes(newRowVersion);
                    entry.Property("RowVersion").CurrentValue = newRowVersion;
                }
            }
        }
    }
}
