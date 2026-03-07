using MediatR;
using CRM.Payment.Application.Features.PaymentTypes.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Payment.Application.Features.PaymentTypes.Queries
{
    public class GetPaymentTypeListQuery : IRequest<PagedResult<PaymentTypeDto>>
    {
        public PaymentTypeFilterDto Filter { get; set; } = new();
    }
}
