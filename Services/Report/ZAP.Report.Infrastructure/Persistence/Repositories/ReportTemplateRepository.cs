using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Report.Domain.Entities;
using ZAP.Report.Domain.Interfaces;

namespace ZAP.Report.Infrastructure.Persistence.Repositories
{
    public class ReportTemplateRepository : BaseMongoRepository<ReportTemplate>, IMongoRepository<ReportTemplate>
    {
        public ReportTemplateRepository(MongoDbContext context, ICurrentUserService currentUserService) 
            : base(context.Database, "ReportTemplates", currentUserService)
        {
        }
    }
}
