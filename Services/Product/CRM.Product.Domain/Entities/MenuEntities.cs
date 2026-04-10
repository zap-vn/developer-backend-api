using System;
using System.Collections.Generic;

namespace CRM.Product.Domain.Entities
{
    public class MenuHeader
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string menu_type { get; set; } = "DIGITAL";
        public Guid? app_id { get; set; }
        public int? status_id { get; set; }
        public string? timezone_id { get; set; }
        public bool is_active { get; set; } = true;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? StatusLabel { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("status_id")]
        public virtual StatusItem? status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int TotalItems { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int SectionsCount { get; set; }

        public virtual ICollection<MenuSection> sections { get; set; } = new List<MenuSection>();
        public virtual ICollection<MenuAvailabilitySchedule> schedules { get; set; } = new List<MenuAvailabilitySchedule>();
    }

    public class MenuSection
    {
        public Guid id { get; set; }
        public Guid menu_id { get; set; }
        public string name { get; set; } = string.Empty;
        public int sort_order { get; set; }

        public virtual MenuHeader menu { get; set; } = null!;
    }

    public class MenuAvailabilitySchedule
    {
        public Guid id { get; set; }
        public Guid menu_id { get; set; }
        public int day_of_week { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }

        public virtual MenuHeader menu { get; set; } = null!;
    }
}
