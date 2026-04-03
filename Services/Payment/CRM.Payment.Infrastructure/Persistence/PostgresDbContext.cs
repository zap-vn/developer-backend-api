using Microsoft.EntityFrameworkCore;
using System;

namespace CRM.Payment.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        public DbSet<PaymentTransaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.ToTable("payment_transaction", "commerce");
                entity.HasKey(e => e.id);
            });
        }
    }

    public class PaymentTransaction
    {
        public Guid id { get; set; }
        public Guid tenant_id { get; set; }
        public Guid order_id { get; set; }
        public string order_number { get; set; } = string.Empty;
        public decimal amount_captured { get; set; }
        public string payment_method { get; set; } = string.Empty;
        public Guid? provider_id { get; set; }
        public string? provider_tx_id { get; set; }
        public int status_id { get; set; }
        public DateTime processed_at { get; set; }
    }
}
