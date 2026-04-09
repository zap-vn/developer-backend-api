using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using MediatR;

namespace CRM.Product.Application.Features.Menus.Commands
{
    public class MenuCommandHandlers : 
        IRequestHandler<CreateMenuCommand, Guid>,
        IRequestHandler<UpdateMenuCommand, bool>,
        IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IMenuRepository _repository;

        public MenuCommandHandlers(IMenuRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = new MenuHeader
            {
                id = Guid.NewGuid(),
                tenant_id = request.tenant_id,
                name = request.name,
                menu_type = request.menu_type,
                timezone_id = request.timezone_id,
                is_active = request.is_active,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            await _repository.CreateAsync(menu);
            return menu.id;
        }

        public async Task<bool> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _repository.GetByIdAsync(request.id);
            if (menu == null) return false;

            menu.name = request.name;
            menu.menu_type = request.menu_type;
            menu.timezone_id = request.timezone_id;
            menu.is_active = request.is_active;
            menu.updated_at = DateTime.UtcNow;

            await _repository.UpdateAsync(menu);
            return true;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _repository.GetByIdAsync(request.id);
            if (menu == null) return false;

            await _repository.DeleteAsync(request.id);
            return true;
        }
    }
}
