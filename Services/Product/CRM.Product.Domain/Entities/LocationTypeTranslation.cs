using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("lookup_translations", Schema = "system")]
    public class LocationTypeTranslation
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("lookup_id")]
        public int lookup_id { get; set; }

        [Column("locale_id")]
        public int locale_id { get; set; }

        [Column("name")]
        public string name { get; set; } = string.Empty;

        public virtual LocationTypeItem? location_type { get; set; }
    }
}
