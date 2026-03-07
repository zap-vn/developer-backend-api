using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Sales.Domain.Entities.Reports;
using CRM.Sales.Domain.Interfaces;

namespace CRM.Sales.Infrastructure.Persistence.Repositories
{
    public class ReportTemplateRepository : BaseMongoRepository<ReportTemplate>, IMongoRepository<ReportTemplate>
    {
        public ReportTemplateRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "ReportTemplates", currentUserService)
        {
        }
    }
}
