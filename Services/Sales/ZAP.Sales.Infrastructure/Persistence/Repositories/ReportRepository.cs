using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Sales.Domain.Entities.Reports;
using ZAP.Sales.Domain.Interfaces;
using ZAP.Sales.Application.Common.Interfaces;
using ZAP.Sales.Application.Features.Reports.DTOs;

namespace ZAP.Sales.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : BaseMongoRepository<ReportTemplate>, IReportRepository
    {
        public ReportRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "ReportTemplates", currentUserService)
        {
        }

        public async Task<SalesSummaryDto> GetOverviewListLocationAsync(ReportRequestDto request, Guid userGuid)
        {
            // Mock implementation to fix build
            return await Task.FromResult(new SalesSummaryDto());
        }
    }
}
