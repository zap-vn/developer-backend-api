using MediatR;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetProductListQuery : IRequest<PagedResult<ProductDto>>
    {
        public FilterDTOs Filter { get; set; } = new();
    }
}
