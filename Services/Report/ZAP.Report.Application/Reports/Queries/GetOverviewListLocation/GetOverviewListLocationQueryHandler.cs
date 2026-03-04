using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ZAP.Report.Application.Common.Interfaces;
using ZAP.Report.Application.Reports.DTOs;

namespace ZAP.Report.Application.Reports.Queries.GetOverviewListLocation
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
