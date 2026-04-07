using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("store", Schema = "pos")]
    public class Store
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("location_id")]
        public Guid? location_id { get; set; }

        [Column("legacy_id")]
        public string? legacy_id { get; set; }

        [Column("store_code")]
        public string? store_code { get; set; }

        [Column("store_name")]
        public string? store_name { get; set; }

        [Column("address_line_1")]
        public string? address_line_1 { get; set; }

        [Column("phone_number")]
        public string? phone_number { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("business_address_id")]
        public Guid? business_address_id { get; set; }

        [Column("node_type_id")]
        public int? node_type_id { get; set; }

        [Column("country_id")]
        public int? country_id { get; set; }

        [Column("province_id")]
        public int? province_id { get; set; }

        [Column("district_id")]
        public int? district_id { get; set; }

        [Column("ward_id")]
        public int? ward_id { get; set; }

        [Column("timezone")]
        public string? timezone { get; set; }

        [Column("opening_time")]
        public TimeSpan? opening_time { get; set; }

        [Column("closing_time")]
        public TimeSpan? closing_time { get; set; }

        [Column("status_id")]
        public int? status_id { get; set; }

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

        [ForeignKey("location_id")]
        public Location? location { get; set; }

        [ForeignKey("status_id")]
        public StatusItem? status { get; set; }
        
        // Extended fields from the old Location model if needed
        [NotMapped]
        public string? nickname { get; set; }
        [NotMapped]
        public string? description { get; set; }
        [NotMapped]
        public string? website { get; set; }
        [NotMapped]
        public string? x_link { get; set; }
        [NotMapped]
        public string? instagram_link { get; set; }
        [NotMapped]
        public string? facebook_link { get; set; }
        [NotMapped]
        public string? logo_url { get; set; }
        [NotMapped]
        public string? brand_color { get; set; }
        [NotMapped]
        public string? business_hours { get; set; }
        [NotMapped]
        public string? preferred_language { get; set; }
    }
}
