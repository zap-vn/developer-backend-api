using Microsoft.EntityFrameworkCore;
using ProductEntity = CRM.Product.Domain.Entities.Product;
using VariantEntity = CRM.Product.Domain.Entities.ProductVariant;
using CategoryEntity = CRM.Product.Domain.Entities.Category;
using BrandEntity = CRM.Product.Domain.Entities.Brand;
using ModifierGroupEntity = CRM.Product.Domain.Entities.ModifierGroup;
using ProductMediaEntity = CRM.Product.Domain.Entities.ProductMedia;
using CategoryMappingEntity = CRM.Product.Domain.Entities.ProductCategoryMap;
using LocationPriceEntity = CRM.Product.Domain.Entities.ProductLocationPricing;
using StatusItemEntity = CRM.Product.Domain.Entities.StatusItem;
using StatusTranslationEntity = CRM.Product.Domain.Entities.StatusItemTranslation;
using LocationEntity = CRM.Product.Domain.Entities.Location;
using UomItemEntity = CRM.Product.Domain.Entities.UomItem;
using CRM.Product.Domain.Entities;
using InventoryItemEntity = CRM.Product.Domain.Entities.InventoryItem;
using BomHeaderEntity = CRM.Product.Domain.Entities.BomHeader;

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
        public DbSet<ProductMediaEntity> ProductMedia { get; set; }
        public DbSet<CategoryMappingEntity> ProductCategoryMappings { get; set; }
        public DbSet<LocationPriceEntity> ProductLocationPricing { get; set; }
        public DbSet<StatusItemEntity> StatusItems { get; set; }
        public DbSet<StatusTranslationEntity> StatusItemTranslations { get; set; }
        public DbSet<LocationEntity> Locations { get; set; }
        public DbSet<LocationTypeItem> LocationTypeItems { get; set; }
        public DbSet<Store> Stores { get; set; }

        public DbSet<UomItemEntity> UomItems { get; set; }
        public DbSet<ProductTypeItem> ProductTypeItems { get; set; }
        public DbSet<InventoryItemEntity> InventoryItems { get; set; }
        public DbSet<BomHeaderEntity> BomHeaders { get; set; }
        public DbSet<CRM.Product.Domain.Entities.ProvinceItem> ProvinceItems { get; set; }
        public DbSet<CRM.Product.Domain.Entities.ProvinceTranslation> ProvinceTranslations { get; set; }
        public DbSet<LocationTypeTranslation> LocationTypeTranslations { get; set; }
        
        // --- Catalog Menu System ---
        public DbSet<MenuHeader> MenuHeaders { get; set; }
        public DbSet<MenuSection> MenuSections { get; set; }
        public DbSet<MenuAvailabilitySchedule> MenuSchedules { get; set; }
        public DbSet<MenuItemHd> MenuItems { get; set; }
        public DbSet<MenuPriceOverride> MenuPriceOverrides { get; set; }

        // --- Dining Options ---
        public DbSet<DiningOption> DiningOptions { get; set; }
        public DbSet<DiningOptionTranslation> DiningOptionTranslations { get; set; }
        public DbSet<DiningOptionLocationLink> DiningOptionLocationLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("unaccent");
            modelBuilder.Entity<MenuItemHd>(entity =>
            {
                entity.ToTable("menu_item_hd", "catalog");
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.variant).WithMany().HasForeignKey(e => e.product_variant_id);
            });

            modelBuilder.Entity<MenuPriceOverride>(entity =>
            {
                entity.ToTable("menu_price_override", "catalog");
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.variant).WithMany().HasForeignKey(e => e.product_variant_id);
            });
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.ToTable("product", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.brand_id).HasColumnName("brand_id");
                entity.Property(e => e.product_type_id).HasColumnName("product_type_id");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.legacy_id).HasColumnName("legacy_id");
                entity.Property(e => e.short_description).HasColumnName("short_description");
                entity.Property(e => e.long_description_html).HasColumnName("long_description_html");
                entity.Property(e => e.search_vector).HasColumnName("search_vector");
                entity.Property(e => e.priority_score).HasColumnName("priority_score");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.is_featured).HasColumnName("is_featured");
                entity.Property(e => e.created_at).HasColumnName("created_at");
                entity.Property(e => e.updated_at).HasColumnName("updated_at");
                
                entity.HasMany(e => e.variants)
                    .WithOne(v => v.product)
                    .HasForeignKey(v => v.product_id)
                    .IsRequired(false);

                entity.HasMany(e => e.category_mappings)
                    .WithOne(cm => cm.product)
                    .HasForeignKey(cm => cm.product_id);

                entity.HasOne(e => e.status)
                    .WithMany()
                    .HasForeignKey(e => e.status_id);

                entity.HasOne(e => e.product_type)
                    .WithMany()
                    .HasForeignKey(e => e.product_type_id);
            });

            modelBuilder.Entity<VariantEntity>(entity =>
            {
                entity.ToTable("product_variant", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.product_id).HasColumnName("product_id");
                entity.Property(e => e.sku_code).HasColumnName("sku_code");
                entity.Property(e => e.barcode).HasColumnName("barcode");
                entity.Property(e => e.variant_name).HasColumnName("variant_name");
                entity.Property(e => e.base_price).HasColumnName("base_price");
                entity.Property(e => e.sale_price).HasColumnName("sale_price");
                entity.Property(e => e.cost_price).HasColumnName("cost_price");
                entity.Property(e => e.is_default).HasColumnName("is_default");
                entity.Property(e => e.attributes).HasColumnName("attributes");
                entity.Property(e => e.weight_grams).HasColumnName("weight_grams");
                entity.Property(e => e.length_mm).HasColumnName("length_mm");
                entity.Property(e => e.width_mm).HasColumnName("width_mm");
                entity.Property(e => e.height_mm).HasColumnName("height_mm");
                entity.Property(e => e.uom_id).HasColumnName("uom_id");
                
                entity.HasOne(e => e.uom).WithMany().HasForeignKey(e => e.uom_id);

                entity.HasMany(e => e.media)
                    .WithOne(m => m.variant)
                    .HasForeignKey(m => m.product_variant_id);

                entity.HasMany(e => e.location_pricing)
                    .WithOne(lp => lp.variant)
                    .HasForeignKey(lp => lp.product_variant_id);

                entity.HasMany(e => e.inventory_items)
                    .WithOne(i => i.Variant)
                    .HasForeignKey(i => i.product_variant_id);

                entity.HasMany(e => e.bom_headers)
                    .WithOne(b => b.variant)
                    .HasForeignKey(b => b.product_variant_id);
            });

            modelBuilder.Entity<ProductMediaEntity>(entity =>
            {
                entity.ToTable("product_media", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.product_variant_id).HasColumnName("product_variant_id");
                entity.Property(e => e.media_url).HasColumnName("media_url");
                entity.Property(e => e.is_primary).HasColumnName("is_primary");
                entity.Property(e => e.sort_order).HasColumnName("sort_order");
            });

            modelBuilder.Entity<CategoryMappingEntity>(entity =>
            {
                entity.ToTable("product_category_assignment", "catalog");
                entity.HasKey(e => new { e.product_id, e.category_id });
                entity.Property(e => e.product_id).HasColumnName("product_id");
                entity.Property(e => e.category_id).HasColumnName("category_id");
                entity.Property(e => e.is_primary).HasColumnName("is_primary");

                entity.HasOne(e => e.category)
                    .WithMany(e => e.product_mappings)
                    .HasForeignKey(e => e.category_id);
            });

            modelBuilder.Entity<LocationPriceEntity>(entity =>
            {
                entity.ToTable("product_location_pricing", "catalog");
                entity.HasKey(e => new { e.product_variant_id, e.location_id });
                entity.Property(e => e.product_variant_id).HasColumnName("product_variant_id");
                entity.Property(e => e.location_id).HasColumnName("location_id");
                entity.Property(e => e.sale_price_override).HasColumnName("sale_price_override");
            });

            modelBuilder.Entity<StatusItemEntity>(entity =>
            {
                entity.ToTable("status_item", "system");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.code).HasColumnName("code");
                
                entity.HasMany(e => e.translations)
                    .WithOne(t => t.status_item)
                    .HasForeignKey(t => t.status_item_id);
            });

            modelBuilder.Entity<ProductTypeItem>(entity =>
            {
                entity.ToTable("product_type_item", "platform");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.code).HasColumnName("code");
                
                entity.HasMany(e => e.translations).WithOne().HasForeignKey(e => e.product_type_id);
            });

            modelBuilder.Entity<ProductTypeTranslation>(entity =>
            {
                entity.ToTable("product_type_translation", "platform");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.product_type_id).HasColumnName("product_type_id");
                entity.Property(e => e.locale_id).HasColumnName("locale_id");
                entity.Property(e => e.name).HasColumnName("name");
            });

            modelBuilder.Entity<StatusTranslationEntity>(entity =>
            {
                entity.ToTable("status_item_translation", "system");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.status_item_id).HasColumnName("status_item_id");
                entity.Property(e => e.locale_id).HasColumnName("locale_id");
                entity.Property(e => e.name).HasColumnName("name");
            });

            modelBuilder.Entity<CRM.Product.Domain.Entities.ProvinceItem>(entity =>
            {
                entity.ToTable("province_item", "platform");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.code).HasColumnName("code");
                entity.Property(e => e.is_active).HasColumnName("is_active");
                
                entity.HasMany(e => e.translations)
                    .WithOne(t => t.province_item)
                    .HasForeignKey(t => t.province_id);
            });

            modelBuilder.Entity<CRM.Product.Domain.Entities.ProvinceTranslation>(entity =>
            {
                entity.ToTable("province_translation", "platform");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.province_id).HasColumnName("province_id");
                entity.Property(e => e.locale_id).HasColumnName("locale_id");
                entity.Property(e => e.name).HasColumnName("name");
            });

            modelBuilder.Entity<LocationEntity>(entity =>
            {
                entity.ToTable("location", "commerce");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.serial_id).HasColumnName("serial_id");
                entity.Property(e => e.serial_number).HasColumnName("serial_number");
                entity.Property(e => e.location_code).HasColumnName("location_code");
                entity.Property(e => e.node_id).HasColumnName("node_id");
                entity.Property(e => e.legacy_id).HasColumnName("legacy_id");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.is_active).HasColumnName("is_active");
                entity.Property(e => e.created_at).HasColumnName("created_at");
                entity.Property(e => e.updated_at).HasColumnName("updated_at");
                entity.Property(e => e.slug).HasColumnName("slug");
                entity.Property(e => e.business_name).HasColumnName("business_name");
                entity.Property(e => e.description).HasColumnName("description");
                entity.Property(e => e.location_type_id).HasColumnName("location_type_id");
                entity.Property(e => e.address_line_1).HasColumnName("address_line_1");
                entity.Property(e => e.address_line_2).HasColumnName("address_line_2");
                entity.Property(e => e.city).HasColumnName("city");
                entity.Property(e => e.state).HasColumnName("state");
                entity.Property(e => e.country_id).HasColumnName("country_id");
                entity.Property(e => e.province_id).HasColumnName("province_id");
                entity.Property(e => e.district_id).HasColumnName("district_id");
                entity.Property(e => e.ward_id).HasColumnName("ward_id");
                entity.Property(e => e.zipcode).HasColumnName("zipcode");
                entity.Property(e => e.phone_number).HasColumnName("phone_number");
                entity.Property(e => e.email).HasColumnName("email");
                entity.Property(e => e.website).HasColumnName("website");
                entity.Property(e => e.twitter).HasColumnName("twitter");
                entity.Property(e => e.instagram).HasColumnName("instagram");
                entity.Property(e => e.facebook).HasColumnName("facebook");
                entity.Property(e => e.logo_url).HasColumnName("logo_url");
                entity.Property(e => e.cover_image_url).HasColumnName("cover_image_url");
                entity.Property(e => e.brand_color).HasColumnName("brand_color");
                entity.Property(e => e.timezone).HasColumnName("timezone");
                entity.Property(e => e.latitude).HasColumnName("latitude");
                entity.Property(e => e.longitude).HasColumnName("longitude");
                entity.Property(e => e.operating_hours).HasColumnName("operating_hours").HasColumnType("jsonb");
                entity.Property(e => e.transfer_account).HasColumnName("transfer_account");
                entity.Property(e => e.transfer_tag).HasColumnName("transfer_tag");
                entity.Property(e => e.parent_location_id).HasColumnName("parent_location_id");
            });

            modelBuilder.Entity<LocationTypeItem>(entity =>
            {
                entity.ToTable("lookups", "system");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.code).HasColumnName("code");

                entity.HasMany(e => e.translations).WithOne(t => t.location_type).HasForeignKey(t => t.lookup_id);
            });

            modelBuilder.Entity<LocationTypeTranslation>(entity =>
            {
                entity.ToTable("lookup_translations", "system");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.lookup_id).HasColumnName("lookup_id");
                entity.Property(e => e.locale_id).HasColumnName("locale_id");
                entity.Property(e => e.name).HasColumnName("name");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("store", "pos");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.location_id).HasColumnName("location_id");
                entity.Property(e => e.legacy_id).HasColumnName("legacy_id");
                entity.Property(e => e.store_code).HasColumnName("store_code");
                entity.Property(e => e.store_name).HasColumnName("store_name");
                entity.Property(e => e.address_line_1).HasColumnName("address_line_1");
                entity.Property(e => e.phone_number).HasColumnName("phone_number");
                entity.Property(e => e.email).HasColumnName("email");
                entity.Property(e => e.business_address_id).HasColumnName("business_address_id");
                entity.Property(e => e.node_type_id).HasColumnName("node_type_id");
                entity.Property(e => e.country_id).HasColumnName("country_id");
                entity.Property(e => e.province_id).HasColumnName("province_id");
                entity.Property(e => e.district_id).HasColumnName("district_id");
                entity.Property(e => e.ward_id).HasColumnName("ward_id");
                entity.Property(e => e.timezone).HasColumnName("timezone");
                entity.Property(e => e.opening_time).HasColumnName("opening_time");
                entity.Property(e => e.closing_time).HasColumnName("closing_time");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                entity.Property(e => e.created_at).HasColumnName("created_at");
                entity.Property(e => e.updated_at).HasColumnName("updated_at");

                entity.HasOne(e => e.location)
                    .WithMany()
                    .HasForeignKey(e => e.location_id);
            });

            modelBuilder.Entity<UomItem>(entity =>
            {
                entity.ToTable("uom_item", "platform");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.code).HasColumnName("code");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.abbreviation).HasColumnName("abbreviation");
                entity.Property(e => e.precision).HasColumnName("precision");
                entity.Property(e => e.status_id).HasColumnName("status_id");
                
                entity.HasOne(e => e.status).WithMany().HasForeignKey(e => e.status_id);
                entity.HasMany(e => e.translations).WithOne().HasForeignKey(e => e.uom_item_id);
            });

            modelBuilder.Entity<UomItemTranslation>(entity =>
            {
                entity.ToTable("uom_item_translation", "platform");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.uom_item_id).HasColumnName("uom_item_id");
                entity.Property(e => e.locale_id).HasColumnName("locale_id");
                entity.Property(e => e.name).HasColumnName("name");
            });

            modelBuilder.Entity<InventoryItemEntity>(entity =>
            {
                entity.ToTable("inventory_item", "logistics");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.product_variant_id).HasColumnName("product_variant_id");
                entity.Property(e => e.location_id).HasColumnName("location_id");
                entity.Property(e => e.qty_on_hand).HasColumnName("qty_on_hand");

                entity.HasOne(e => e.Location)
                    .WithMany()
                    .HasForeignKey(e => e.location_id);
            });

            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.ToTable("category", "catalog");
                entity.HasKey(e => e.id);

                entity.Property(e => e.serial_id).HasColumnName("serial_id");
                entity.Property(e => e.serial_number).HasColumnName("serial_number");
                entity.Property(e => e.category_code).HasColumnName("category_code");
                entity.Property(e => e.meta_keywords).HasColumnName("meta_keywords");
                entity.Property(e => e.canonical_url).HasColumnName("canonical_url");
                entity.Property(e => e.display_initial).HasColumnName("display_initial");
                entity.Property(e => e.applicable_channels).HasColumnName("applicable_channels");

                entity.HasOne(e => e.parent_category)
                    .WithMany(e => e.sub_categories)
                    .HasForeignKey(e => e.parent_id);

                entity.HasOne(e => e.status)
                    .WithMany()
                    .HasForeignKey(e => e.status_id);
            });
            modelBuilder.Entity<BomHeaderEntity>(entity =>
            {
                entity.ToTable("bom_header", "catalog");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.product_variant_id).HasColumnName("product_variant_id");
                entity.Property(e => e.uom_id).HasColumnName("uom_id");
                entity.Property(e => e.is_active).HasColumnName("is_active");
            });

            modelBuilder.Entity<BrandEntity>(entity => { entity.ToTable("brand", "catalog"); entity.HasKey(e => e.id); });
            modelBuilder.Entity<ModifierGroupEntity>(entity => { entity.ToTable("modifier_group", "catalog"); entity.HasKey(e => e.id); });

            // --- Dining Options ---
            modelBuilder.Entity<DiningOption>(entity =>
            {
                entity.ToTable("dining_option", "commerce");
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<DiningOptionTranslation>(entity =>
            {
                entity.ToTable("dining_option_translation", "commerce");
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<DiningOptionLocationLink>(entity =>
            {
                entity.ToTable("location_dining_option", "commerce");
                entity.HasKey(e => new { e.dining_option_id, e.location_id });
            });
        }
    }
}
