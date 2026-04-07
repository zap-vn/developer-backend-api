using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ModifierGroupListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("filters")]
        public ModifierGroupListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public ModifierGroupListSortDto Sort { get; set; } = new();
    }

    public class ModifierGroupListFilterDto
    {
        /// <summary>Filter by status_id. Null = all.</summary>
        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }

        /// <summary>
        /// Filter by display type derived from max_selection.
        /// "single" = max_selection == 1 | "multi" = max_selection > 1 | null = all.
        /// </summary>
        [JsonPropertyName("display_type")]
        public string? DisplayType { get; set; }
    }

    /// <summary>Sort config. field: "name" (default) | "sort_order"</summary>
    public class ModifierGroupListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "name";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = false;
    }
}
