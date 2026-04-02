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
                Id = x.id,
                ParentId = x.parent_id,
                Name = x.name,
                IsActive = true,
                IconUrl = x.icon_url,
                MaterializedPath = x.materialized_path,
                SeoTitle = x.seo_title
            }).ToList();

            // Build hierarchy
            var lookup = allDtos.ToLookup(x => x.ParentId);
            foreach (var dto in allDtos)
            {
                dto.Children = lookup[dto.Id].ToList();
            }

            return allDtos.Where(x => x.ParentId == null);
        }
    }
}
