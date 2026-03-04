using System.Threading.Tasks;
using ZAP.Report.Application.Reports.DTOs;

namespace ZAP.Report.Application.Common.Interfaces
{
    public interface IReportRepository
    {
        Task<SalesSummaryDto> GetOverviewListLocationAsync(ReportRequestDto request, string userGuid);
    }
}
