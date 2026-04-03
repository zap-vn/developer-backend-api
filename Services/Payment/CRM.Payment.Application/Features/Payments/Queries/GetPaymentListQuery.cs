using MediatR;
using CRM.BuildingBlocks.Models;
using CRM.Payment.Application.Features.Payments.DTOs;
using System;

namespace CRM.Payment.Application.Features.Payments.Queries
{
    public class GetPaymentListQuery : IRequest<PagedResult<PaymentListDto>>
    {
        public Guid? ProviderId { get; set; }
        public int? StatusId { get; set; }
        public string? TransactionRef { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
