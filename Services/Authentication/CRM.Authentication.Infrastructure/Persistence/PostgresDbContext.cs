using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TenantNode> TenantNodes { get; set; }
        public DbSet<CustomerOtp> Otps { get; set; }
        public DbSet<PasswordResetRequest> PasswordResets { get; set; }
        public DbSet<StatusItem> StatusItems { get; set; }
        public DbSet<Locale> Locales { get; set; }
        public DbSet<EmailSetting> EmailSettings { get; set; }
        // SystemConfig removed
        public DbSet<SystemError> SystemErrors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatusItem>(entity =>
            {
                entity.ToTable("status", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.code).HasColumnName("code");
                entity.Property(e => e.domain).HasColumnName("domain");
            });

            modelBuilder.Entity<Locale>(entity =>
            {
                entity.ToTable("locale", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.code).HasColumnName("code");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.legacy_id).HasColumnName("legacy_id").HasMaxLength(128);
                entity.Property(e => e.username).HasColumnName("username").HasMaxLength(128).IsRequired();
                entity.Property(e => e.email).HasColumnName("email").HasMaxLength(128).IsRequired();
                entity.Property(e => e.password_hash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.full_name).HasColumnName("full_name").HasMaxLength(255).IsRequired();
                entity.Property(e => e.status_id).HasColumnName("status_id").HasDefaultValue(9001);
                
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.Property(e => e.updated_at).HasColumnName("updated_at").HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<TenantNode>(entity =>
            {
                entity.ToTable("tenant_node", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.parent_id).HasColumnName("parent_id");
                entity.Property(e => e.node_code).HasColumnName("node_code").IsRequired();
                entity.Property(e => e.legacy_id).HasColumnName("legacy_id");
                entity.Property(e => e.tier_level).HasColumnName("tier_level").IsRequired();
                entity.Property(e => e.name).HasColumnName("name").IsRequired();
                entity.Property(e => e.slug).HasColumnName("slug").IsRequired();
                entity.Property(e => e.locale_id).HasColumnName("locale_id");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.avatar_id).HasColumnName("avatar_id");
                entity.Property(e => e.banner_id).HasColumnName("banner_id");
                entity.Property(e => e.address_line_1).HasColumnName("address_line_1");
                entity.Property(e => e.country_id).HasColumnName("country_id");
                entity.Property(e => e.province_id).HasColumnName("province_id");
                entity.Property(e => e.district_id).HasColumnName("district_id");
                entity.Property(e => e.ward_id).HasColumnName("ward_id");
                entity.Property(e => e.zipcode).HasColumnName("zipcode");
                entity.Property(e => e.timezone).HasColumnName("timezone");
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.Property(e => e.updated_at).HasColumnName("updated_at").HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<CustomerOtp>(entity =>
            {
                entity.ToTable("otp", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.email).HasColumnName("email");
                entity.Property(e => e.phone).HasColumnName("phone");
                entity.Property(e => e.otp_code).HasColumnName("otp_code").IsRequired();
                entity.Property(e => e.purpose).HasColumnName("purpose");
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.Property(e => e.expired_at).HasColumnName("expired_at");
                entity.Property(e => e.verified_at).HasColumnName("verified_at");
                entity.Property(e => e.customer_id).HasColumnName("customer_id");
            });

            modelBuilder.Entity<PasswordResetRequest>(entity =>
            {
                entity.ToTable("password_reset", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.user_guid).HasColumnName("user_guid");
                entity.Property(e => e.email).HasColumnName("email").IsRequired();
                entity.Property(e => e.phone).HasColumnName("phone");
                entity.Property(e => e.method).HasColumnName("method");
                entity.Property(e => e.channel).HasColumnName("channel");
                entity.Property(e => e.otp_hash).HasColumnName("otp_hash");
                entity.Property(e => e.token).HasColumnName("token").IsRequired();
                entity.Property(e => e.confirm_token).HasColumnName("confirm_token");
                entity.Property(e => e.attempts).HasColumnName("attempts").HasDefaultValue(0);
                entity.Property(e => e.is_used).HasColumnName("is_used").HasDefaultValue(false);
                entity.Property(e => e.expired_at).HasColumnName("expired_at");
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<EmailSetting>(entity =>
            {
                entity.ToTable("email_setting", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.customer_guid).HasColumnName("customer_guid");
                entity.Property(e => e.smtp_host).HasColumnName("smtp_host");
                entity.Property(e => e.smtp_port).HasColumnName("smtp_port");
                entity.Property(e => e.smtp_user).HasColumnName("smtp_user");
                entity.Property(e => e.smtp_pass).HasColumnName("smtp_pass");
                entity.Property(e => e.from_email).HasColumnName("from_email");
                entity.Property(e => e.from_name).HasColumnName("from_name");
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<SystemError>(entity =>
            {
                entity.ToTable("system_error", "identity");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.message).HasColumnName("message");
                entity.Property(e => e.detail).HasColumnName("detail");
                entity.Property(e => e.source).HasColumnName("source");
                entity.Property(e => e.user_id).HasColumnName("user_id");
                entity.Property(e => e.merchant_name).HasColumnName("merchant_name");
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("now()");
            });
        }
    }
}

