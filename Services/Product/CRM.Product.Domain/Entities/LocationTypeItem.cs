using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("location_type_item", Schema = "pos")]
    public class LocationTypeItem
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("code")]
        public string? code { get; set; }

        [Column("label_en")]
        public string? label_en { get; set; }

        [Column("label_vi")]
        public string? label_vi { get; set; }

        [Column("is_active")]
        public bool? is_active { get; set; }
    }
}
