using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Queries
{
    public record GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync();
            
            var allDtos = categories.Select(x => new CategoryDto
            {
                id = x.id,
                parent_id = x.parent_id,
                name = x.name,
                is_active = x.is_active,
                icon_url = x.icon_url,
                materialized_path = x.materialized_path,
                seo_title = x.seo_title
            }).ToList();

            // Build hierarchy
            var lookup = allDtos.ToLookup(x => x.parent_id);
            foreach (var dto in allDtos)
            {
                dto.children = lookup[dto.id].ToList();
            }

            return allDtos.Where(x => x.parent_id == null).ToList();
        }
    }
}
