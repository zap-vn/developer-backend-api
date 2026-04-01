using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateModifierGroupCommandHandler : IRequestHandler<CreateModifierGroupCommand, Guid>
    {
        private readonly IModifierGroupRepository _repository;

        public CreateModifierGroupCommandHandler(IModifierGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateModifierGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = new ModifierGroup
            {
                tenant_id = request.TenantId,
                legacy_id = request.LegacyId,
                name = request.Name,
                min_selection = request.MinSelection,
                max_selection = request.MaxSelection,
                is_required = request.IsRequired,
                sort_order = request.SortOrder
            };

            await _repository.CreateAsync(entity);
            return entity.id;
        }
    }
}
