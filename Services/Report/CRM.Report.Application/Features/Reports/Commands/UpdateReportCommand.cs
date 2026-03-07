using MediatR;
using System;

namespace CRM.Report.Application.Features.Reports.Commands
{
    public class UpdateReportCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty; // Injected from route
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ConfigurationJson { get; set; } = string.Empty;
    }
}
