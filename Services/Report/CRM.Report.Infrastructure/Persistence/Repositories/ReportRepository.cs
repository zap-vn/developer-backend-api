using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Report.Domain.Entities;
using CRM.Report.Domain.Interfaces;

namespace CRM.Report.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : BaseMongoRepository<ReportTemplate>, IReportRepository
    {
        public ReportRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "report.ReportTemplates", currentUserService)
        {
        }
    }
}
