using MediatR;
using CRM.Report.Application.Features.Reports.DTOs;
using System.Threading;
using System.Threading.Tasks;
using CRM.Report.Domain.Interfaces;

namespace CRM.Report.Application.Features.Reports.Queries
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
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new ReportDto 
            { 
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Type = entity.Type,
                ConfigurationJson = entity.ConfigurationJson
            };
        }
    }
}
