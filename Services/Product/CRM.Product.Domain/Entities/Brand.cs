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
        public Guid? tenant_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string slug { get; set; } = string.Empty;
        public string logo_url { get; set; } = string.Empty;
        public string banner_url { get; set; } = string.Empty;
        public string website_url { get; set; } = string.Empty;
        public int status_id { get; set; } = 2101;
        public bool is_premium { get; set; } = false;

        /// <summary>Legal / display vendor name (may differ from brand name).</summary>
        public string? vendor_name { get; set; }

        /// <summary>Contact phone number.</summary>
        public string? phone_number { get; set; }

        /// <summary>Contact email address.</summary>
        public string? email_address { get; set; }

        [ForeignKey("status_id")]
        public StatusItem? status { get; set; }
    }
}
