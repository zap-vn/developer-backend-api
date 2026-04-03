using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CRM.Promotion.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        public DbSet<PromotionHeader> Promotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PromotionHeader>(entity =>
            {
                entity.ToTable("promotion", "marketing");
                entity.HasKey(e => e.id);
            });
        }
    }

    public class PromotionHeader
    {
        public Guid id { get; set; }
        public Guid tenant_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string discount_type { get; set; } = "PERCENTAGE";
        public decimal discount_value { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public bool is_active { get; set; } = true;
    }
}
