namespace CRM.BuildingBlocks.Models
{
    public class LookupDto
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? Description { get; set; }
        public object? ExtraData { get; set; }
    }
}
