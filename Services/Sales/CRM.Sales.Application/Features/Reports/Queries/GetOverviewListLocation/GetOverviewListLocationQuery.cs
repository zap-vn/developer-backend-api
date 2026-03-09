using MediatR;
using CRM.Sales.Application.Features.Reports.DTOs;

namespace CRM.Sales.Application.Features.Reports.Queries.GetOverviewListLocation
{
    public class GetOverviewListLocationQuery : IRequest<SalesSummaryDto>
    {
        public ReportRequestDto Request { get; set; } = new();
        public string UserGuid { get; set; } = string.Empty;
    }
}
