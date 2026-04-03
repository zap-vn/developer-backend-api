using Microsoft.EntityFrameworkCore;
using CRM.Customer.Domain.Entities;

namespace CRM.Customer.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<LoyaltyTier> LoyaltyTiers { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<MembershipSubscription> MembershipSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerEntity>(entity =>
            {
                entity.ToTable("customer", "people");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.legacy_id).HasColumnName("legacy_id");
                entity.Property(e => e.phone_number).HasColumnName("phone_number");
                entity.Property(e => e.email).HasColumnName("email");
                entity.Property(e => e.full_name).HasColumnName("full_name");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.tier_id).HasColumnName("tier_id");
                entity.Property(e => e.group_id).HasColumnName("group_id");

                entity.HasOne(e => e.loyalty_tier)
                    .WithMany(t => t.customers)
                    .HasForeignKey(e => e.tier_id);
            });

            modelBuilder.Entity<LoyaltyTier>(entity =>
            {
                entity.ToTable("loyalty_tier", "people");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.tier_name).HasColumnName("tier_name");
            });

            modelBuilder.Entity<MembershipPlan>(entity =>
            {
                entity.ToTable("membership_plan", "people");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.plan_name).HasColumnName("plan_name");
            });

            modelBuilder.Entity<MembershipSubscription>(entity =>
            {
                entity.ToTable("membership_subscription", "people");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.customer_id).HasColumnName("customer_id");
                entity.Property(e => e.plan_id).HasColumnName("plan_id");

                entity.HasOne(e => e.customer)
                    .WithMany()
                    .HasForeignKey(e => e.customer_id);

                entity.HasOne(e => e.plan)
                    .WithMany()
                    .HasForeignKey(e => e.plan_id);
            });
        }
    }

    public class MembershipPlan
    {
        public Guid id { get; set; }
        public string plan_name { get; set; } = string.Empty;
        public string? description { get; set; }
        public bool is_active { get; set; } = true;
    }

    public class MembershipSubscription
    {
        public Guid id { get; set; }
        public Guid customer_id { get; set; }
        public Guid plan_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public bool auto_renew { get; set; }
        public int status_id { get; set; }

        public virtual CustomerEntity customer { get; set; } = null!;
        public virtual MembershipPlan plan { get; set; } = null!;
    }
}
