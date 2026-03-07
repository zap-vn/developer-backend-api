using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Report.Domain.Entities;
using CRM.Report.Domain.Interfaces;

namespace CRM.Report.Application.Features.Reports.Commands
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, string>
    {
        private readonly IReportRepository _repository;

        public CreateReportCommandHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            var entity = new ReportTemplate
            {
                Code = request.Code,
                Name = request.Name,
                Type = request.Type,
                ConfigurationJson = request.ConfigurationJson
            };

            await _repository.CreateAsync(entity);
            return entity.Id.ToString();
        }
    }
}
