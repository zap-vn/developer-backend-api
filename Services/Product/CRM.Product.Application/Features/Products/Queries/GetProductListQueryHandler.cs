using MediatR;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CRM.Product.Domain.Interfaces;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public GetProductListQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync();
            var dtos = list.Select(x => new ProductDto 
            { 
#pragma warning disable CS8602
                Id = x.Id.ToString(),
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Stock = x.Stock,
                Category = x.Category,
                ImageUrl = x.ImageUrl
#pragma warning restore CS8602
            }).ToList();

            return new PagedResult<ProductDto>(dtos, dtos.Count, request.Filter.Page, request.Filter.PageSize);
        }
    }
}
