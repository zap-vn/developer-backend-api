using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Report.Domain.Interfaces;

namespace ZAP.Report.Application.Features.Reports.Commands
{
    public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand, bool>
    {
        private readonly IReportRepository _repository;

        public UpdateReportCommandHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return false;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return false;

            entity.Name = request.Name;
            entity.Type = request.Type;
            entity.ConfigurationJson = request.ConfigurationJson;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
