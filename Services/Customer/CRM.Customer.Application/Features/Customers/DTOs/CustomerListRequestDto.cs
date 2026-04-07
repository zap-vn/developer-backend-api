using System.Text.Json.Serialization;

namespace CRM.Customer.Application.Features.Customers.DTOs
{
    public class CustomerListRequestDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 20;

        /// <summary>Searches across full_name, phone_number, legacy_id, and id (Guid).</summary>
        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("filters")]
        public CustomerListFilterDto Filters { get; set; } = new();

        [JsonPropertyName("sort")]
        public CustomerListSortDto Sort { get; set; } = new();
    }

    public class CustomerListFilterDto
    {
        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }

        [JsonPropertyName("tier_id")]
        public int? TierId { get; set; }

        /// <summary>Filter: total_spent_amount >= value.</summary>
        [JsonPropertyName("min_total_spent")]
        public decimal? MinTotalSpent { get; set; }

        /// <summary>Filter: total_spent_amount <= value.</summary>
        [JsonPropertyName("max_total_spent")]
        public decimal? MaxTotalSpent { get; set; }

        /// <summary>Filter: current_points_balance >= value.</summary>
        [JsonPropertyName("min_points")]
        public decimal? MinPoints { get; set; }

        /// <summary>Filter: current_points_balance <= value.</summary>
        [JsonPropertyName("max_points")]
        public decimal? MaxPoints { get; set; }

        /// <summary>Placeholder for wallet balance filter — no current_balance column yet.</summary>
        [JsonPropertyName("min_balance")]
        public decimal? MinBalance { get; set; }

        /// <summary>Placeholder for wallet balance filter — no current_balance column yet.</summary>
        [JsonPropertyName("max_balance")]
        public decimal? MaxBalance { get; set; }
    }

    /// <summary>
    /// Sort config.
    /// field: "full_name" (default) | "total_spent_amount" | "current_points_balance" | "tier_id" | "status_id"
    /// </summary>
    public class CustomerListSortDto
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = "full_name";

        [JsonPropertyName("descending")]
        public bool Descending { get; set; } = false;
    }
}
