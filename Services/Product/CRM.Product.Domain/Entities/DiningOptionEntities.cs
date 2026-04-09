using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("dining_option", Schema = "platform")]
    public class DiningOption
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("code")]
        public string code { get; set; } = string.Empty;

        [Column("is_active")]
        public bool is_active { get; set; } = true;

        [Column("order_tracking_enabled")]
        public bool order_tracking_enabled { get; set; } = false;

        // Navigation
        public ICollection<DiningOptionTranslation> translations { get; set; } = new List<DiningOptionTranslation>();

        // Transient properties for UI
        [NotMapped]
        public string? DisplayName { get; set; }
        
        [NotMapped]
        public int UsedInLocations { get; set; }
    }

    [Table("dining_option_translation", Schema = "platform")]
    public class DiningOptionTranslation
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("dining_option_id")]
        public int dining_option_id { get; set; }

        [Column("locale_id")]
        public int locale_id { get; set; }

        [Column("name")]
        public string name { get; set; } = string.Empty;

        // Navigation
        [ForeignKey("dining_option_id")]
        public DiningOption? dining_option { get; set; }
    }

    [Table("dining_option_location_link", Schema = "platform")]
    public class DiningOptionLocationLink
    {
        [Column("dining_option_id")]
        public int dining_option_id { get; set; }

        [Column("location_id")]
        public Guid location_id { get; set; }

        [Column("is_active")]
        public bool is_active { get; set; } = true;
    }
}
