using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _repository;

        public CreateCategoryCommandHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = new Category
            {
                id = Guid.NewGuid(),
                tenant_id = request.tenant_id,
                parent_id = request.parent_id,
                legacy_id = request.legacy_id,
                name = request.name,
                slug = request.slug,
                icon_url = request.icon_url,
                banner_url = request.banner_url,
                sort_order = request.sort_order,
                meta_title = request.meta_title,
                meta_description = request.meta_description,
                status_id = request.status_id,
                is_active = request.is_active,
                seo_title = request.seo_title,
                seo_description = request.seo_description
            };

            await _repository.CreateAsync(entity);
            return entity.id;
        }
    }
}
