using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryRepository _repository;

        public GetCategoryByIdQueryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var x = await _repository.GetByIdAsync(request.Id);
            if (x == null) return null;

            return new CategoryDto
            {
                id = x.id,
                parent_id = x.parent_id,
                legacy_id = x.legacy_id,
                materialized_path = x.materialized_path,
                name = x.name,
                slug = x.slug,
                icon_url = x.icon_url,
                banner_url = x.banner_url,
                sort_order = x.sort_order ?? 0,
                meta_title = x.meta_title,
                meta_description = x.meta_description,
                status_id = x.status_id,
                is_active = x.is_active,
                seo_title = x.seo_title,
                seo_description = x.seo_description
            };
        }
    }
}
