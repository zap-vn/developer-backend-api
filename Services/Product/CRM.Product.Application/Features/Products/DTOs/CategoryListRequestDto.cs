using CRM.BuildingBlocks.Models;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class CategoryListRequestDto : PaginationDto
    {
        public string? SearchTerm { get; set; }
    }
}
