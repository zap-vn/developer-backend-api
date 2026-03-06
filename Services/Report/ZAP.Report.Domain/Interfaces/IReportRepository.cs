using ZAP.BuildingBlocks.Interfaces;
using ZAP.Report.Domain.Entities;

namespace ZAP.Report.Domain.Interfaces
{
    public interface IReportRepository : IMongoRepository<ReportTemplate>
    {
    }
}
