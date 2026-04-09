using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("geo_province", Schema = "platform")]
    public class GeoProvince
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("code")]
        public string code { get; set; } = string.Empty;

        [Column("is_active")]
        public bool is_active { get; set; } = true;

        public ICollection<GeoProvinceTranslation> translations { get; set; } = new List<GeoProvinceTranslation>();
    }
}
