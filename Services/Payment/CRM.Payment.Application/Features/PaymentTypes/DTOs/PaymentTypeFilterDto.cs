using CRM.BuildingBlocks.Models;

namespace CRM.Payment.Application.Features.PaymentTypes.DTOs
{
    public class PaymentTypeFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
    }
}
