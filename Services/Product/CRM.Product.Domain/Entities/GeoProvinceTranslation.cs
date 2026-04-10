using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("geo_province_translation", Schema = "platform")]
    public class GeoProvinceTranslation
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; }

        [Column("province_id")]
        public int province_id { get; set; }

        [Column("locale_id")]
        public int locale_id { get; set; }

        [Column("name")]
        public string name { get; set; } = string.Empty;

        public GeoProvince? province_item { get; set; }
    }
}
