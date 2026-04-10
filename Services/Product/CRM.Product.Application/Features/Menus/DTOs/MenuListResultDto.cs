using System;

namespace CRM.Product.Application.Features.Menus.DTOs
{
    public class MenuListResultDto
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string menu_type { get; set; } = string.Empty;
        public string channel => menu_type; // Match SQL alias
        public string? app_id { get; set; }
        public int? status_id { get; set; }
        public string? status_code { get; set; }
        public string? status_name { get; set; }
        public string? timezone_id { get; set; }
        public bool is_active { get; set; }
        public int sections_count { get; set; }
        public int total_items { get; set; }
    }
}
