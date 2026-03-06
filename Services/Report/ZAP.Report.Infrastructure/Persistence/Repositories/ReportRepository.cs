using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Report.Domain.Entities;
using ZAP.Report.Domain.Interfaces;

namespace ZAP.Report.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : BaseMongoRepository<ReportTemplate>, IReportRepository
    {
        public ReportRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "ReportTemplates", currentUserService)
        {
        }
    }
}
