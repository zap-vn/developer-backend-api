using MediatR;
using CRM.Payment.Application.Features.PaymentTerms.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Payment.Application.Features.PaymentTerms.Queries
{
    public class GetPaymentTermsListQuery : IRequest<PagedResult<PaymentTermsDto>>
    {
        public FilterDTOs Filter { get; set; } = new();
    }
}
