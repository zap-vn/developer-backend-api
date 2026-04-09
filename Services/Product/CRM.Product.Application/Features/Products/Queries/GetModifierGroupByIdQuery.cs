using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetModifierGroupByIdQuery : IRequest<ModifierGroupDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetModifierGroupByIdQueryHandler : IRequestHandler<GetModifierGroupByIdQuery, ModifierGroupDto?>
    {
        private readonly IModifierGroupRepository _repository;

        public GetModifierGroupByIdQueryHandler(IModifierGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<ModifierGroupDto?> Handle(GetModifierGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var x = await _repository.GetByIdAsync(request.Id);
            if (x == null) return null;

            return new ModifierGroupDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                legacy_id = x.legacy_id,
                name = x.name,
                min_selection = x.min_selection,
                max_selection = x.max_selection,
                is_required = x.is_required,
                sort_order = x.sort_order,
                status_id = x.status_id
            };
        }
    }
}
