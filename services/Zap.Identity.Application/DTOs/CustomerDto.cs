using System.Text.Json.Serialization;

namespace Zap.Identity.Application.DTOs;

public class CustomerDto
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public string? CustomerCode { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? BusinessName { get; set; }
    public string? MerchantName { get; set; }
    public string? ProjectName { get; set; }
    public string? BusinessType { get; set; }
    public bool UseAiContentGeneration { get; set; }
    public string? Language { get; set; }
    public string? DateFormat { get; set; }
    public string? TimeFormat { get; set; }
    public string? TimeZoneId { get; set; }
    public string? TimeZoneDisplayName { get; set; }
    public int? Country { get; set; }
    public List<string> ReferenceAssets { get; set; } = new();
    public string? Phone { get; set; }
    public int CustomerStatusId { get; set; }
}
