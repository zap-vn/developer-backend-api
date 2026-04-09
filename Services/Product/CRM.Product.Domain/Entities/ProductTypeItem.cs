using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("product_type_item", Schema = "platform")]
    public class ProductTypeItem
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("code")]
        public string? code { get; set; }

        public System.Collections.Generic.ICollection<ProductTypeTranslation> translations { get; set; } = new System.Collections.Generic.List<ProductTypeTranslation>();
    }

    [Table("product_type_translation", Schema = "platform")]
    public class ProductTypeTranslation
    {
        [Key]
        public System.Guid id { get; set; }
        public int product_type_id { get; set; }
        public int locale_id { get; set; }
        public string? name { get; set; }
    }
}
