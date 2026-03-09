using MediatR;
using CRM.Report.Application.Features.Reports.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Report.Application.Features.Reports.Queries
{
    public class GetReportListQuery : IRequest<PagedResult<ReportDto>>
    {
        public FilterDTOs Filter { get; set; } = new();
    }
}
