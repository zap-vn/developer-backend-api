using CRM.BuildingBlocks.Interfaces;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Guid>
    {
        private readonly IUnitRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CreateUnitCommandHandler(IUnitRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            var tenantIdString = _currentUserService.UserGuid;
            Guid tenantId = Guid.Empty;
            if (Guid.TryParse(tenantIdString, out var guid)) tenantId = guid;

            var entity = new UomItem
            {
                id = Guid.NewGuid(),
                tenant_id = tenantId,
                name = request.Name,
                code = request.Code,
                uom_type = request.UomType ?? "UNIT"
            };

            await _repository.CreateAsync(entity);
            return entity.id;
        }
    }
}
