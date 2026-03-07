using MediatR;
using System;

namespace CRM.Report.Application.Features.Reports.Commands
{
    public class CreateReportCommand : IRequest<string>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ConfigurationJson { get; set; } = string.Empty;
    }
}
