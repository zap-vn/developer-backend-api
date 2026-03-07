using System;

namespace CRM.Report.Application.Features.Reports.DTOs
{
    public class ReportDto
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ConfigurationJson { get; set; } = string.Empty;
    }
}
