using System.Text.Json.Serialization;

namespace CRM.Sales.Application.Features.Reports.DTOs
{
    public class ReportResponseDto<T>
    {
        [JsonPropertyName("Success")]
        public bool Success { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("Model")]
        public T Model { get; set; } = default!;
    }

    public class OverviewResponse
    {
        [JsonPropertyName("Overview")]
        public SalesSummaryDto Overview { get; set; } = default!;
    }
}
