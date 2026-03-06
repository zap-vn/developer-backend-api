using MediatR;
using ZAP.Payment.Application.Features.PaymentTypes.DTOs;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Payment.Application.Features.PaymentTypes.Queries
{
    public class GetPaymentTypeListQuery : IRequest<PagedResult<PaymentTypeDto>>
    {
        public PaymentTypeFilterDto Filter { get; set; } = new();
    }
}
