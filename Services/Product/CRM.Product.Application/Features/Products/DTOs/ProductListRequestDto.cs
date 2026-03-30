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
    }

    public class ProductListFilterDto
    {
        [JsonPropertyName("cate_id")]
        public List<string> CateId { get; set; } = new();

        [JsonPropertyName("status")]
        public List<int> Status { get; set; } = new();
    }
}
