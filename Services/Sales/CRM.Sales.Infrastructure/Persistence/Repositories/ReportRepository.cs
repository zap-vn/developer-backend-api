using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Sales.Domain.Entities.Reports;
using CRM.Sales.Domain.Interfaces;
using CRM.Sales.Application.Common.Interfaces;
using CRM.Sales.Application.Features.Reports.DTOs;

namespace CRM.Sales.Infrastructure.Persistence.Repositories
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
