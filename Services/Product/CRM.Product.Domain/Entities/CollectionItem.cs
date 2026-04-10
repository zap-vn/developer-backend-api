using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("collection_item", Schema = "catalog")]
    public class CollectionItem
    {
        [Column("collection_id")]
        public Guid collection_id { get; set; }

        [Column("product_id")]
        public Guid product_id { get; set; }

        [Column("sort_order")]
        public int sort_order { get; set; } = 0;

        public Collection? collection { get; set; }
    }
}
