using System;
using System.Collections.Generic;

namespace CRM.Customer.Domain.Entities
{
    public class StatusItem
    {
        public int id { get; set; }
        public int? group_id { get; set; }
        public string? domain { get; set; }
        public string? code { get; set; }
        public int? sort_order { get; set; }

        public virtual ICollection<StatusItemTranslation> translations { get; set; } = new List<StatusItemTranslation>();
    }

    public class StatusItemTranslation
    {
        public Guid id { get; set; }
        public int status_item_id { get; set; }
        public int locale_id { get; set; }
        public string? name { get; set; }

        public virtual StatusItem? status_item { get; set; }
    }
}
