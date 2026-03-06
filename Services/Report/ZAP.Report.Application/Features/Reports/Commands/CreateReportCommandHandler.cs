using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Report.Domain.Entities;
using ZAP.Report.Domain.Interfaces;

namespace ZAP.Report.Application.Features.Reports.Commands
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
