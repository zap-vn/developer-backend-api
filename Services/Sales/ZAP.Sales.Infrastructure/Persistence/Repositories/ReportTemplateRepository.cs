using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Sales.Domain.Entities.Reports;
using ZAP.Sales.Domain.Interfaces;

namespace ZAP.Sales.Infrastructure.Persistence.Repositories
{
    public class ReportTemplateRepository : BaseMongoRepository<ReportTemplate>, IMongoRepository<ReportTemplate>
    {
        public ReportTemplateRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "ReportTemplates", currentUserService)
        {
        }
    }
}
