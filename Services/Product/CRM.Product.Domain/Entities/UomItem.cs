using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("uom_item", Schema = "platform")]
    public class UomItem
    {
        [Key]
        public int id { get; set; }
        public Guid tenant_id { get; set; }
        public string code { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string uom_type { get; set; } = "STOCK"; // STOCK, SELL
    }
}
