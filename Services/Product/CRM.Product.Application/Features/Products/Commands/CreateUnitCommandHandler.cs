using CRM.BuildingBlocks.Interfaces;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, int>
    {
        private readonly IUnitRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CreateUnitCommandHandler(IUnitRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            var tenantIdString = _currentUserService.UserGuid;
            Guid tenantId = Guid.Empty;
            if (Guid.TryParse(tenantIdString, out var guid)) tenantId = guid;

            var entity = new UomItem
            {
                tenant_id = tenantId,
                name = request.Name,
                code = request.Code
            };

            await _repository.CreateAsync(entity);
            return entity.id;
        }
    }
}
