using System;

namespace ZAP.Customer.Application.Features.CustomerGroups.DTOs
{
    public class CustomerGroupDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
    }
}
