using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using MediatR;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetCategoryListQuery : IRequest<PagedResult<CategoryDto>>
    {
        public CategoryListRequestDto Request { get; set; } = new();
    }
}
