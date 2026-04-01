using CRM.BuildingBlocks.Models;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class UnitListRequestDto : PaginationDto
    {
        public string? SearchTerm { get; set; }
        public string? UomType { get; set; }
    }
}
