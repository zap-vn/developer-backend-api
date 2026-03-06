using MediatR;
using ZAP.Report.Application.Features.Reports.DTOs;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Report.Application.Features.Reports.Queries
{
    public class GetReportListQuery : IRequest<PagedResult<ReportDto>>
    {
        public ReportFilterDto Filter { get; set; } = new();
    }
}
