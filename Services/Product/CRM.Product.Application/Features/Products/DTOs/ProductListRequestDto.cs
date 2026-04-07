using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ProductListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int Page { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("search")]
        public string Search { get; set; } = string.Empty;

        [JsonPropertyName("filters")]
        public ProductListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public ProductListSortDto Sort { get; set; } = new();
    }

    public class ProductListFilterDto
    {
        [JsonPropertyName("cate_id")]
        public string? CategoryId { get; set; }

        [JsonPropertyName("status")]
        public int? StatusId { get; set; }

        [JsonPropertyName("location_id")]
        public string? LocationId { get; set; }

        [JsonPropertyName("locale_id")]
        public int? LocaleId { get; set; }

        [JsonPropertyName("product_type_id")]
        public int? ProductTypeId { get; set; }
    }

    /// <summary>
    /// Sort config. field: "name" | "price" | "stock" | "created_at" (default)
    /// </summary>
    public class ProductListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "created_at";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = true;
    }
}
