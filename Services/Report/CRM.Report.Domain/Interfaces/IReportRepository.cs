using CRM.BuildingBlocks.Interfaces;
using CRM.Report.Domain.Entities;

namespace CRM.Report.Domain.Interfaces
{
    public interface IReportRepository : IMongoRepository<ReportTemplate>
    {
    }
}
