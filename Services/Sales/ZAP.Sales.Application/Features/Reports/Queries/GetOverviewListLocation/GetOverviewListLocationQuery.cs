using MediatR;
using ZAP.Sales.Application.Features.Reports.DTOs;

namespace ZAP.Sales.Application.Features.Reports.Queries.GetOverviewListLocation
{
    public class GetOverviewListLocationQuery : IRequest<SalesSummaryDto>
    {
        public ReportRequestDto Request { get; set; } = new();
        public Guid UserGuid { get; set; }
    }
}
