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
    }
}
