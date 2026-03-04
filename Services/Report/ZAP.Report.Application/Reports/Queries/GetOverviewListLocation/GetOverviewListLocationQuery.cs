using MediatR;
using ZAP.Report.Application.Reports.DTOs;

namespace ZAP.Report.Application.Reports.Queries.GetOverviewListLocation
{
    public class GetOverviewListLocationQuery : IRequest<SalesSummaryDto>
    {
        public ReportRequestDto Request { get; set; } = new();
        public string UserGuid { get; set; } = string.Empty;
    }
}
