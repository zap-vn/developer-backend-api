using Microsoft.EntityFrameworkCore;
using System;

namespace CRM.Order.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderHeader> Orders { get; set; }
        public DbSet<OrderStatusInfo> StatusItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderHeader>(entity =>
            {
                entity.ToTable("order_header", "commerce");
                entity.HasKey(e => e.id);
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.order_number).HasColumnName("order_number");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.total_amount).HasColumnName("total_amount");
                entity.Property(e => e.channel).HasColumnName("order_channel");
                entity.Property(e => e.created_at).HasColumnName("created_at");
            });

            modelBuilder.Entity<OrderStatusInfo>(entity =>
            {
                entity.ToTable("status_item", "platform");
                entity.HasKey(e => e.id);
            });
        }
    }

    public class OrderHeader
    {
        public Guid id { get; set; }
        public Guid tenant_id { get; set; }
        public string order_number { get; set; } = string.Empty;
        public int status_id { get; set; }
        public decimal total_amount { get; set; }
        public string? channel { get; set; }
        public string? customer_name { get; set; }
        public DateTime created_at { get; set; }
    }

    public class OrderStatusInfo
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
    }
}
