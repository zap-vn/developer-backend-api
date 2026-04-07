using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class CategoryListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("filters")]
        public CategoryListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public CategoryListSortDto Sort { get; set; } = new();
    }

    public class CategoryListFilterDto
    {
        /// <summary>Filter by status_id (e.g. 1=Active, 0=Inactive). Null = all.</summary>
        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }

        /// <summary>Filter by sales channel. Placeholder — no channel table yet.</summary>
        [JsonPropertyName("channel_id")]
        public int? ChannelId { get; set; }
    }

    /// <summary>
    /// Sort config. field: "name" (default) | "sort_order"
    /// </summary>
    public class CategoryListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "name";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = false;
    }
}
