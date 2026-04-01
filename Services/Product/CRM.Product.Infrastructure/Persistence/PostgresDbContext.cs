using Microsoft.EntityFrameworkCore;
using ProductEntity = CRM.Product.Domain.Entities.Product;
using VariantEntity = CRM.Product.Domain.Entities.ProductVariant;
using CategoryEntity = CRM.Product.Domain.Entities.Category;
using BrandEntity = CRM.Product.Domain.Entities.Brand;
using ModifierGroupEntity = CRM.Product.Domain.Entities.ModifierGroup;
// UomItem removed — platform.uom_item table does not exist in current DB

namespace CRM.Product.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<VariantEntity> ProductVariants { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<BrandEntity> Brands { get; set; }
        public DbSet<ModifierGroupEntity> ModifierGroups { get; set; }
        // UomItems DbSet removed — platform.uom_item table does not exist

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.ToTable("product", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.brand_id).HasColumnName("brand_id");
                entity.Property(e => e.legacy_id).HasColumnName("legacy_id");
                entity.Property(e => e.product_type).HasColumnName("product_type").HasMaxLength(16);
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(255).IsRequired();
                entity.Property(e => e.short_description).HasColumnName("short_description");
                entity.Property(e => e.long_description_html).HasColumnName("long_description_html");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.is_featured).HasColumnName("is_featured").HasDefaultValue(false);

                entity.HasMany(e => e.variants)
                    .WithOne(v => v.product)
                    .HasForeignKey(v => v.product_id);
            });

            modelBuilder.Entity<VariantEntity>(entity =>
            {
                entity.ToTable("product_variant", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.product_id).HasColumnName("product_id").IsRequired();
                entity.Property(e => e.sku_code).HasColumnName("sku_code").HasMaxLength(64);
                entity.HasIndex(e => e.sku_code).IsUnique();
                entity.Property(e => e.barcode).HasColumnName("barcode").HasMaxLength(64);
                entity.Property(e => e.variant_name).HasColumnName("variant_name");
                entity.Property(e => e.base_price).HasColumnName("base_price").HasColumnType("numeric(19,4)");
                entity.Property(e => e.sale_price).HasColumnName("sale_price").HasColumnType("numeric(19,4)");
                entity.Property(e => e.cost_price).HasColumnName("cost_price").HasColumnType("numeric(19,4)");
                // Columns that do NOT exist in catalog.product_variant
                entity.Ignore(e => e.tenant_id);
                entity.Ignore(e => e.is_active);
                entity.Ignore(e => e.stock_quantity);
                entity.Ignore(e => e.unit_of_measure);
                entity.Ignore(e => e.weight_grams);
                entity.Ignore(e => e.length_mm);
                entity.Ignore(e => e.width_mm);
                entity.Ignore(e => e.height_mm);
                entity.Ignore(e => e.attributes);
            });
            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.ToTable("category", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.parent_id).HasColumnName("parent_id");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(255).IsRequired();
                entity.Ignore(e => e.is_active);
                entity.Property(e => e.icon_url).HasColumnName("icon_url");
                entity.Property(e => e.materialized_path).HasColumnName("materialized_path");
                entity.Property(e => e.seo_title).HasColumnName("seo_title");
                entity.Property(e => e.seo_description).HasColumnName("seo_description");
                entity.Ignore(e => e.channels);

                entity.HasOne(e => e.Parent)
                    .WithMany(p => p.Children)
                    .HasForeignKey(e => e.parent_id);
            });

            modelBuilder.Entity<BrandEntity>(entity =>
            {
                entity.ToTable("brand", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
                entity.HasIndex(e => e.slug).IsUnique();
            });

            modelBuilder.Entity<ModifierGroupEntity>(entity =>
            {
                entity.ToTable("modifier_group", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            });

            // UomItem mapping removed — platform.uom_item does not exist in current DB
        }
    }
}

