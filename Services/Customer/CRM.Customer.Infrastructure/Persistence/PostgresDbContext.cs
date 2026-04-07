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
                entity.Property(e => e.gender).HasColumnName("gender");
                entity.Property(e => e.birth_date).HasColumnName("birth_date");
                entity.Property(e => e.country_id).HasColumnName("country_id");
                entity.Property(e => e.province_id).HasColumnName("province_id");
                entity.Property(e => e.district_id).HasColumnName("district_id");
                entity.Property(e => e.ward_id).HasColumnName("ward_id");
                entity.Property(e => e.zipcode).HasColumnName("zipcode");
                entity.Property(e => e.preferred_locale_id).HasColumnName("preferred_locale_id");
                entity.Property(e => e.user_id).HasColumnName("user_id");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.tier_id).HasColumnName("tier_id");
                entity.Property(e => e.group_id).HasColumnName("group_id");
                entity.Property(e => e.current_points_balance).HasColumnName("current_points_balance");
                entity.Property(e => e.total_spent_amount).HasColumnName("total_spent_amount");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.loyalty_tier)
                    .WithMany(t => t.customers)
                    .HasForeignKey(e => e.tier_id);

                entity.Ignore(e => e.Id);
                entity.Ignore(e => e.UserGuid);
                entity.Ignore(e => e.CreatedBy);
                entity.Ignore(e => e.UpdatedBy);
                entity.Ignore(e => e.IsDeleted);

                // Ignore MongoDB-specific complex types and other non-Postgres fields
                entity.Ignore(e => e.TaxSyncSetting);
                entity.Ignore(e => e._key);
                entity.Ignore(e => e._rev);
                entity.Ignore(e => e.BusinessName);
                entity.Ignore(e => e.BussinessTypeId);
                entity.Ignore(e => e.CurrencyId);
                entity.Ignore(e => e.CurrencyNativeName);
                entity.Ignore(e => e.CurrencySymbol);
                entity.Ignore(e => e.CustomerCode);
                entity.Ignore(e => e.Language);
                entity.Ignore(e => e.FirstName);
                entity.Ignore(e => e.LastName);
                entity.Ignore(e => e.Phone);
                entity.Ignore(e => e.Point);
                entity.Ignore(e => e.MerchantUrl);
                entity.Ignore(e => e.Name);
                entity.Ignore(e => e.Address);
                entity.Ignore(e => e.PhoneNumber);
                entity.Ignore(e => e.IsActive);
                entity.Ignore(e => e.CreateDate);
                entity.Ignore(e => e.StartedDate);
                entity.Ignore(e => e.TimeZoneDisplayName);
                entity.Ignore(e => e.TimeZoneId);
                entity.Ignore(e => e.Websites);
                entity.Ignore(e => e.MerchantName);
                entity.Ignore(e => e.RegistrationSource);
                entity.Ignore(e => e.PublicKey);
                entity.Ignore(e => e.AdminUpdate);
                entity.Ignore(e => e.BatchCode);
                entity.Ignore(e => e.LinkVAT);
                entity.Ignore(e => e.InterestGrade);
                entity.Ignore(e => e.PassCode);
                entity.Ignore(e => e.Password);
                entity.Ignore(e => e.Plural);
                entity.Ignore(e => e.Singular);
                entity.Ignore(e => e.EmpGuid);
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
