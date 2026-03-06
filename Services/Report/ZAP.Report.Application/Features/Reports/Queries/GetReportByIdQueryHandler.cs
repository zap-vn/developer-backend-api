using MediatR;
using ZAP.Report.Application.Features.Reports.DTOs;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Report.Domain.Interfaces;

namespace ZAP.Report.Application.Features.Reports.Queries
{
    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDto>
    {
        private readonly IReportRepository _repository;

        public GetReportByIdQueryHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReportDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return null;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return null;

            return new ReportDto 
            { 
                Id = entity.Id.ToString(),
                Code = entity.Code,
                Name = entity.Name,
                Type = entity.Type,
                ConfigurationJson = entity.ConfigurationJson
            };
        }
    }
}
