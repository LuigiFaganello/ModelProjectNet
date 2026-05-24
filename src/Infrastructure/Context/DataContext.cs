using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Example> Examples => Set<Example>();

        public override int SaveChanges()
        {
            ApplyAuditTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }

        private void ApplyAuditTimestamps()
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(EntityBase.UpdatedDate)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(EntityBase.CreatedDate)).IsModified = false;
                }
            }
        }
    }
}
