namespace CRM.Sales.Application.Features.Reports.DTOs
{
    public class ReportRequestDto
    {
        public bool? IsCurrentDrawer { get; set; }
        public string DrawerId { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string StartDay { get; set; } = string.Empty;
        public string EndDay { get; set; } = string.Empty;
        public string StartHour { get; set; } = string.Empty;
        public string EndHour { get; set; } = string.Empty;
        public bool? IsAllDevices { get; set; }
        public bool? IsAllDay { get; set; }
        public string Location_id { get; set; } = string.Empty;
        public List<string> DeviceIds { get; set; } = new();
    }
}
