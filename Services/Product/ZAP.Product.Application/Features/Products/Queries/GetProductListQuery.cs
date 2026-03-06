using MediatR;
using ZAP.Product.Application.Features.Products.DTOs;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Product.Application.Features.Products.Queries
{
    public class GetProductListQuery : IRequest<PagedResult<ProductDto>>
    {
        public ProductFilterDto Filter { get; set; } = new();
    }
}
