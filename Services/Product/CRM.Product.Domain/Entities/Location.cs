using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("location", Schema = "commerce")]
    public class Location
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("serial_id")]
        public long serial_id { get; set; }

        [Column("serial_number")]
        public string serial_number { get; set; } = string.Empty;

        [Column("location_code")]
        public string location_code { get; set; } = string.Empty;

        [Column("tenant_id")]
        public Guid? tenant_id { get; set; }

        [Column("node_id")]
        public Guid? node_id { get; set; }

        [Column("legacy_id")]
        public string? legacy_id { get; set; }

        [Column("business_name")]
        public string business_name { get; set; } = "Phở 24";

        [Column("name")]
        public string name { get; set; } = string.Empty;

        [Column("slug")]
        public string slug { get; set; } = string.Empty;

        [Column("description")]
        public string? description { get; set; }

        [Column("location_type_id")]
        public int? location_type_id { get; set; }

        [Column("address_line_1")]
        public string? address_line_1 { get; set; }

        [Column("address_line_2")]
        public string? address_line_2 { get; set; }

        [Column("city")]
        public string? city { get; set; }

        [Column("state")]
        public string? state { get; set; }

        [Column("country_id")]
        public int? country_id { get; set; }

        [Column("province_id")]
        public int? province_id { get; set; }

        [Column("district_id")]
        public int? district_id { get; set; }

        [Column("ward_id")]
        public int? ward_id { get; set; }

        [Column("zipcode")]
        public string? zipcode { get; set; }

        [Column("phone_number")]
        public string? phone_number { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("website")]
        public string? website { get; set; }

        [Column("twitter")]
        public string? twitter { get; set; }

        [Column("instagram")]
        public string? instagram { get; set; }

        [Column("facebook")]
        public string? facebook { get; set; }

        [Column("logo_url")]
        public string? logo_url { get; set; }

        [Column("cover_image_url")]
        public string? cover_image_url { get; set; }

        [Column("brand_color")]
        public string? brand_color { get; set; }

        [Column("timezone")]
        public string? timezone { get; set; } = "Asia/Ho Chi Minh";

        [Column("latitude")]
        public decimal? latitude { get; set; }

        [Column("longitude")]
        public decimal? longitude { get; set; }

        [Column("operating_hours")]
        public System.Text.Json.JsonElement? operating_hours { get; set; }

        [Column("transfer_account")]
        public string? transfer_account { get; set; }

        [Column("transfer_tag")]
        public string? transfer_tag { get; set; }

        [Column("parent_location_id")]
        public Guid? parent_location_id { get; set; }

        [Column("status_id")]
        public int status_id { get; set; } = 30001;

        [Column("is_active")]
        public bool is_active { get; set; } = true;

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

        [ForeignKey("location_type_id")]
        public LocationTypeItem? location_type { get; set; }

        [ForeignKey("status_id")]
        public StatusItem? status { get; set; }
    }
}
