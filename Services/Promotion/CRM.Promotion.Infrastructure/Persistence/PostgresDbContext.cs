using Microsoft.EntityFrameworkCore;
using CRM.Promotion.Domain.Entities;
using System;

namespace CRM.Promotion.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        public DbSet<PromotionEntity> Promotions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PromotionEntity>(entity =>
            {
                entity.ToTable("promotion", "marketing");
                entity.HasKey(e => e.id);
                
                // Keep ID column and BaseEntity ID mapping
                entity.Ignore(e => e.Id);
                entity.Ignore(e => e.UserGuid);
                entity.Ignore(e => e.CreatedBy);
                entity.Ignore(e => e.UpdatedBy);
                entity.Ignore(e => e.IsDeleted);
            });
        }
    }
}
