using System;
using CRM.BuildingBlocks.Models;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ProductFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
    }
}
