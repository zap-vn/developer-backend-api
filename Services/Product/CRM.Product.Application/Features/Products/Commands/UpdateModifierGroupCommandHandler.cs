using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateModifierGroupCommandHandler : IRequestHandler<UpdateModifierGroupCommand, bool>
    {
        private readonly IModifierGroupRepository _repository;

        public UpdateModifierGroupCommandHandler(IModifierGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateModifierGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            if (request.Name != null) entity.name = request.Name;
            entity.min_selection = request.MinSelection;
            entity.max_selection = request.MaxSelection;
            entity.is_required = request.IsRequired;
            entity.sort_order = request.SortOrder;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
