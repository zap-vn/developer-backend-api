using MediatR;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using CRM.Product.Domain.Interfaces;
using CRM.Product.Domain.Entities;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetProductListQueryHandler(IProductRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            var filter = request.Filter;
            var currentUserGuid = _currentUserService.UserGuid;
            
            Expression<Func<ProductEntity, bool>> predicate = x => 
                (string.IsNullOrEmpty(currentUserGuid) || x.UserGuid == currentUserGuid) &&
                (string.IsNullOrEmpty(filter.Keyword) || x.Name.Contains(filter.Keyword) || x.Code.Contains(filter.Keyword)) &&
                (string.IsNullOrEmpty(filter.Category) || x.Category == filter.Category) &&
                (!filter.IsActive.HasValue || x.IsActive == filter.IsActive.Value);

            var pagedResult = await _repository.GetPagedAsync(filter.PageIndex, filter.PageSize, predicate);

            var dtos = pagedResult.Items.Select(x => new ProductDto 
            { 
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Stock = x.Stock,
                Category = x.Category,
                ImageUrl = x.ImageUrl,
                IsActive = x.IsActive
            }).ToList();

            return new PagedResult<ProductDto>(dtos, pagedResult.TotalCount, pagedResult.CurrentPage, pagedResult.PageSize);
        }
    }
}
