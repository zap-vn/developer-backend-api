using System.Text.Json.Serialization;

namespace CRM.Promotion.Application.Features.Promotions.DTOs
{
    public class PromotionListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("filters")]
        public PromotionListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public PromotionListSortDto Sort { get; set; } = new();
    }

    public class PromotionListFilterDto
    {
        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }

        /// <summary>3101 PERCENTAGE | 3102 FIXED_AMOUNT | 3103 SPECIFIC_PRICE | 3104 FREE_ITEM</summary>
        [JsonPropertyName("discount_type_id")]
        public int? DiscountTypeId { get; set; }

        /// <summary>Placeholder: no member_level_id column in promotion table yet.</summary>
        [JsonPropertyName("member_level_id")]
        public int? MemberLevelId { get; set; }

        /// <summary>true = applies to all locations; false = location-specific.</summary>
        [JsonPropertyName("is_all_locations")]
        public bool? IsAllLocations { get; set; }

        /// <summary>true = auto-apply; false = requires manual code entry.</summary>
        [JsonPropertyName("is_automatic")]
        public bool? IsAutomatic { get; set; }
    }

    /// <summary>
    /// Sort config. field: "name" (default) | "discount_value" | "status_id"
    /// </summary>
    public class PromotionListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "name";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = false;
    }
}
