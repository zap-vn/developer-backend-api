using System;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Customer.Application.Features.CustomerGroups.DTOs
{
    public class CustomerGroupFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
    }
}
