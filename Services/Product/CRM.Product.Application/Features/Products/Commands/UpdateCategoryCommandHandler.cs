using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ICategoryRepository _repository;

        public UpdateCategoryCommandHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            if (request.name != null) entity.name = request.name;
            if (request.parent_id.HasValue) entity.parent_id = request.parent_id;
            if (request.legacy_id != null) entity.legacy_id = request.legacy_id;
            if (request.slug != null) entity.slug = request.slug;
            if (request.icon_url != null) entity.icon_url = request.icon_url;
            if (request.banner_url != null) entity.banner_url = request.banner_url;
            if (request.sort_order.HasValue) entity.sort_order = request.sort_order;
            if (request.meta_title != null) entity.meta_title = request.meta_title;
            if (request.meta_description != null) entity.meta_description = request.meta_description;
            if (request.status_id.HasValue) entity.status_id = request.status_id;
            if (request.is_active.HasValue) entity.is_active = request.is_active.Value;
            if (request.seo_title != null) entity.seo_title = request.seo_title;
            if (request.seo_description != null) entity.seo_description = request.seo_description;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
