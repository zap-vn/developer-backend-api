#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("status_item", Schema = "platform")]
    public class StatusItem
    {
        [Key]
        public int id { get; set; }

        
        [Column("code")]
        public string? status_code { get; set; }

        public ICollection<StatusItemTranslation> translations { get; set; } = new List<StatusItemTranslation>();
    }

    [Table("status_item_translation", Schema = "platform")]
    public class StatusItemTranslation
    {
        [Key]
        public Guid id { get; set; }
        
        [Column("status_item_id")]
        public int status_item_id { get; set; }
        
        [Column("locale_id")]
        public int locale_id { get; set; }
        
        [Column("name")]
        public string? name { get; set; }

        public StatusItem? status_item { get; set; }
    }
}
