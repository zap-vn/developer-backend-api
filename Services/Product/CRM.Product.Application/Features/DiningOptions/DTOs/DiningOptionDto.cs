namespace CRM.Product.Application.Features.DiningOptions.DTOs
{
    public class DiningOptionDto
    {
        public int id { get; set; }
        public string code { get; set; } = string.Empty;
        public string display_name { get; set; } = string.Empty;
        public int used_in_locations { get; set; }
        public int sort_order { get; set; }
        public bool is_active { get; set; }
    }
}
