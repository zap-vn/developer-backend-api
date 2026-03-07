using MediatR;
using CRM.Sales.Application.Common.Interfaces;
using CRM.Sales.Application.Features.Reports.DTOs;
using CRM.Sales.Domain.Interfaces;

namespace CRM.Sales.Application.Features.Reports.Queries.GetOverviewListLocation
{
    public class GetOverviewListLocationQueryHandler : IRequestHandler<GetOverviewListLocationQuery, SalesSummaryDto>
    {
        private readonly IReportRepository _repository;

        public GetOverviewListLocationQueryHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<SalesSummaryDto> Handle(GetOverviewListLocationQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetOverviewListLocationAsync(request.Request, request.UserGuid);
        }
    }
}
