using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Menus.DTOs
{
    public class MenuListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("filters")]
        public MenuListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public MenuListSortDto Sort { get; set; } = new();
    }

    public class MenuListFilterDto
    {
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("menu_type")]
        public string? MenuType { get; set; }
    }

    public class MenuListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "name";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = false;
    }
}
