using System;
using CRM.BuildingBlocks.Models;

namespace CRM.Customer.Application.Features.CustomerGroups.DTOs
{
    public class CustomerGroupFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
    }
}
