using System;

namespace CRM.Product.Application.Features.Menus.DTOs
{
    public class MenuListResultDto
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string menu_type { get; set; } = string.Empty;
        public string? timezone_id { get; set; }
        public bool is_active { get; set; }
        public int sections_count { get; set; }
    }
}
