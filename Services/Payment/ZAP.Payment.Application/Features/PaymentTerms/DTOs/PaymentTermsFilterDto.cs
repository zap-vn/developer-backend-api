using ZAP.BuildingBlocks.Models;

namespace ZAP.Payment.Application.Features.PaymentTerms.DTOs
{
    public class PaymentTermsFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
    }
}
