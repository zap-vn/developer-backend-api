using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("custom_unit", Schema = "catalog")]
    public class UomItem
    {
        [Key]
        public int id { get; set; }
        public Guid tenant_id { get; set; }
        public string code { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;

        /// <summary>Short symbol displayed in UI, e.g. "Kg", "L", "Box".</summary>
        public string? abbreviation { get; set; }

        /// <summary>Number of decimal places allowed (0–5).</summary>
        public int precision { get; set; } = 0;

        public int? status_id { get; set; }

        // Navigation
        [ForeignKey("status_id")]
        public StatusItem? status { get; set; }
    }
}
