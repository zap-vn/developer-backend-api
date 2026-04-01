using CRM.BuildingBlocks.Models;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class BrandListRequestDto : PaginationDto
    {
        public string? SearchTerm { get; set; }
        public int? StatusId { get; set; }
    }
}
