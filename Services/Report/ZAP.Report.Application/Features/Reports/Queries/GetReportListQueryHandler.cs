using MediatR;
using ZAP.Report.Application.Features.Reports.DTOs;
using ZAP.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ZAP.Report.Domain.Interfaces;

namespace ZAP.Report.Application.Features.Reports.Queries
{
    public class GetReportListQueryHandler : IRequestHandler<GetReportListQuery, PagedResult<ReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetReportListQueryHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ReportDto>> Handle(GetReportListQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync();
            var dtos = list.Select(x => new ReportDto 
            { 
                Id = x.Id.ToString(),
                Code = x.Code,
                Name = x.Name,
                Type = x.Type,
                ConfigurationJson = x.ConfigurationJson
            }).ToList();

            return new PagedResult<ReportDto>(dtos, dtos.Count, request.Filter.Page, request.Filter.PageSize);
        }
    }
}
