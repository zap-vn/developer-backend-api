using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("brand", Schema = "catalog")]
    public class Brand
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("serial_id")]
        public int? serial_id { get; set; }

        public Guid? tenant_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string slug { get; set; } = string.Empty;
        public string logo_url { get; set; } = string.Empty;
        public string banner_url { get; set; } = string.Empty;
        public string website_url { get; set; } = string.Empty;
        public int status_id { get; set; } = 2101;
        public bool is_premium { get; set; } = false;

        [ForeignKey("status_id")]
        public StatusItem? status { get; set; }
    }
}
