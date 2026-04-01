using CRM.BuildingBlocks.Models;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ModifierGroupListRequestDto : PaginationDto
    {
        public string? SearchTerm { get; set; }
        public bool? IsRequired { get; set; }
    }
}
