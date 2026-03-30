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
            var req = request.Request;
            var currentUserGuid = _currentUserService.UserGuid;
            
            Expression<Func<ProductEntity, bool>> predicate = x => 
                (string.IsNullOrEmpty(currentUserGuid) || x.UserGuid == currentUserGuid || x.EmpGuid == currentUserGuid) &&
                (string.IsNullOrEmpty(req.Search) || (x.Name != null && x.Name.Contains(req.Search)) || (x.Code != null && x.Code.Contains(req.Search)) || (x.Barcode != null && x.Barcode.Contains(req.Search))) &&
                (req.Filters == null || req.Filters.CateId == null || !req.Filters.CateId.Any() || req.Filters.CateId.Contains(x.Category)) &&
                (req.Filters == null || req.Filters.Status == null || !req.Filters.Status.Any() || req.Filters.Status.Contains(x.Visible));

            var pagedResult = await _repository.GetPagedAsync(req.Page, req.PageSize, predicate);

            var dtos = pagedResult.Items.Select(x => new ProductDto 
            { 
                Id = x.Id,
                CateName = x.Category ?? string.Empty,
                MerchantId = x.UserGuid ?? string.Empty,
                Name = x.Name ?? string.Empty,
                Description = x.Description ?? string.Empty,
                Price = x.Price,
                ImageUrl = x.ImageUrl ?? string.Empty,
                Status = x.Visible
            }).ToList();

            return new PagedResult<ProductDto>(dtos, pagedResult.TotalCount, pagedResult.CurrentPage, pagedResult.PageSize);
        }
    }
}
