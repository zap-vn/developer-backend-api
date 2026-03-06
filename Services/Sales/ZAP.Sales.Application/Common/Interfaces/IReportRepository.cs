using System;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.Sales.Domain.Entities.Reports;
using ZAP.Sales.Application.Features.Reports.DTOs;

namespace ZAP.Sales.Application.Common.Interfaces
{
    public interface IReportRepository : IMongoRepository<ReportTemplate>
    {
        Task<SalesSummaryDto> GetOverviewListLocationAsync(ReportRequestDto request, Guid userGuid);
    }
}
