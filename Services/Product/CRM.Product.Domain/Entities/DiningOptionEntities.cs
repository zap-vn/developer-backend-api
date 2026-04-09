using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("dining_option", Schema = "commerce")]
    public class DiningOption
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("code")]
        public string code { get; set; } = string.Empty;

        [Column("is_active")]
        public bool is_active { get; set; }

        [Column("sort_order")]
        public int sort_order { get; set; }

        // Non-mapped properties for DTOs
        [NotMapped]
        public string? DisplayName { get; set; }
        
        [NotMapped]
        public int UsedInLocations { get; set; }

        public ICollection<DiningOptionTranslation> translations { get; set; } = new List<DiningOptionTranslation>();
    }

    [Table("dining_option_translation", Schema = "commerce")]
    public class DiningOptionTranslation
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; }

        [Column("dining_option_id")]
        public int dining_option_id { get; set; }

        [Column("locale_id")]
        public int locale_id { get; set; }

        [Column("name")]
        public string name { get; set; } = string.Empty;
    }

    [Table("location_dining_option", Schema = "commerce")]
    public class DiningOptionLocationLink
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; }

        [Column("location_id")]
        public Guid location_id { get; set; }

        [Column("dining_option_id")]
        public int dining_option_id { get; set; }

        [Column("custom_label")]
        public string? custom_label { get; set; }

        [Column("is_enabled")]
        public bool is_enabled { get; set; }

        [Column("available_on_pos")]
        public bool available_on_pos { get; set; }

        [Column("available_online")]
        public bool available_online { get; set; }

        [Column("sort_order")]
        public int sort_order { get; set; }

        [Column("created_at")]
        public DateTime created_at { get; set; }
    }
}
