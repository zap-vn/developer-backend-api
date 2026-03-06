using MediatR;
using ZAP.Payment.Application.Features.PaymentTerms.DTOs;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Payment.Application.Features.PaymentTerms.Queries
{
    public class GetPaymentTermsListQuery : IRequest<PagedResult<PaymentTermsDto>>
    {
        public PaymentTermsFilterDto Filter { get; set; } = new();
    }
}
