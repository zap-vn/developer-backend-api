using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using MediatR;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetBrandsQuery : IRequest<PagedResult<BrandDto>>
    {
        public BrandListRequestDto Request { get; set; } = new();
    }
}
