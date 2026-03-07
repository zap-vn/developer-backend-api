using CRM.BuildingBlocks.Models;

namespace CRM.Payment.Application.Features.PaymentTerms.DTOs
{
    public class PaymentTermsFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
    }
}
