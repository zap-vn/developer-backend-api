using System;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Product.Application.Features.Products.DTOs
{
    public class ProductFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
    }
}
