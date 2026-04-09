using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.DiningOptions.Commands
{
    public class CreateDiningOptionCommandHandler : IRequestHandler<CreateDiningOptionCommand, int>
    {
        private readonly IDiningOptionRepository _repository;

        public CreateDiningOptionCommandHandler(IDiningOptionRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateDiningOptionCommand request, CancellationToken cancellationToken)
        {
            var entity = new DiningOption
            {
                code = request.Code,
                is_active = request.IsActive,
                order_tracking_enabled = request.OrderTrackingEnabled
            };

            await _repository.CreateAsync(entity);
            return entity.id;
        }
    }
}
