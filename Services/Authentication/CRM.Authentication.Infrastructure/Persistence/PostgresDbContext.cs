using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("identity");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                
                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
                entity.Property(e => e.LegacyId).HasColumnName("legacy_id").HasMaxLength(128);
                entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(128).IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(128).IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
                entity.Property(e => e.StatusId).HasColumnName("status_id").IsRequired(false);
                
                entity.Property(e => e.CreatedAtDate).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.Property(e => e.UpdatedAtDate).HasColumnName("updated_at").HasDefaultValueSql("now()");

                // Ignore Mongo-specific fields if they are in the entity but not in the DB
                entity.Ignore(e => e._id);
                entity.Ignore(e => e._key);
                entity.Ignore(e => e.Password);
                entity.Ignore(e => e.Phone);
                entity.Ignore(e => e.FirstName);
                entity.Ignore(e => e.LastName);
                entity.Ignore(e => e.BusinessName);
                entity.Ignore(e => e.MerchantName);
                entity.Ignore(e => e.Language);
                entity.Ignore(e => e.LanguageId);
                entity.Ignore(e => e.MerchantUrl);
                entity.Ignore(e => e.Provider);
                entity.Ignore(e => e.Acronym);
                entity.Ignore(e => e.Roles);
                entity.Ignore(e => e.Visible);
                entity.Ignore(e => e.IsVerify);
                entity.Ignore(e => e.IsVerifyPhone);
                entity.Ignore(e => e.IsVerifyEmail);
                entity.Ignore(e => e.IsVerifyGoogle);
                entity.Ignore(e => e.IsVerifyApple);
                entity.Ignore(e => e.CreatedAt);
                entity.Ignore(e => e.UpdatedAt);
            });
        }
    }
}
