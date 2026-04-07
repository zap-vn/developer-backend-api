using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class BrandListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Searches across: name (vendor_name), account_number (brand ID),
        /// phone_number, and email_address.
        /// </summary>
        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("filters")]
        public BrandListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public BrandListSortDto Sort { get; set; } = new();
    }

    public class BrandListFilterDto
    {
        /// <summary>Filter by status_id. Null = all.</summary>
        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }
    }

    /// <summary>Sort config. field: "name" (default) | "status"</summary>
    public class BrandListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "name";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = false;
    }
}
