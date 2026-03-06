using MediatR;
using ZAP.Sales.Application.Common.Interfaces;
using ZAP.Sales.Application.Features.Reports.DTOs;
using ZAP.Sales.Domain.Interfaces;

namespace ZAP.Sales.Application.Features.Reports.Queries.GetOverviewListLocation
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
