using System;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.Sales.Domain.Entities.Reports;
using CRM.Sales.Application.Features.Reports.DTOs;

namespace CRM.Sales.Application.Common.Interfaces
{
    public interface IReportRepository : IMongoRepository<ReportTemplate>
    {
        Task<SalesSummaryDto> GetOverviewListLocationAsync(ReportRequestDto request, string userGuid);
    }
}
