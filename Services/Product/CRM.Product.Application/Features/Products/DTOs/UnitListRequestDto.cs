using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class UnitListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        /// <summary>Searches across name and abbreviation (symbol).</summary>
        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("filters")]
        public UnitListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public UnitListSortDto Sort { get; set; } = new();
    }

    public class UnitListFilterDto
    {
        /// <summary>Filter by status_id. Null = all.</summary>
        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }

        /// <summary>Filter by exact decimal precision value (0–5). Null = all.</summary>
        [JsonPropertyName("precision")]
        public int? Precision { get; set; }
    }

    /// <summary>
    /// Sort config. field: "name" (default) | "precision" | "status"
    /// </summary>
    public class UnitListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "name";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = false;
    }
}
