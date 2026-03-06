using MediatR;
using ZAP.Report.Application.Features.Reports.DTOs;

namespace ZAP.Report.Application.Features.Reports.Queries
{
    public class GetReportByIdQuery : IRequest<ReportDto>
    {
        public string Id { get; set; }

        public GetReportByIdQuery(string id)
        {
            Id = id;
        }
    }
}
