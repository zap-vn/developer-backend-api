using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.DiningOptions.Commands
{
    public class UpdateDiningOptionCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public bool? IsActive { get; set; }
        public bool? OrderTrackingEnabled { get; set; }
    }

    public class UpdateDiningOptionCommandHandler : IRequestHandler<UpdateDiningOptionCommand, bool>
    {
        private readonly IDiningOptionRepository _repository;

        public UpdateDiningOptionCommandHandler(IDiningOptionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateDiningOptionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, localeId: null);
            if (entity == null) return false;

            if (request.Code != null) entity.code = request.Code;
            if (request.IsActive.HasValue) entity.is_active = request.IsActive.Value;
            if (request.OrderTrackingEnabled.HasValue) entity.order_tracking_enabled = request.OrderTrackingEnabled.Value;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
