using MediatR;
using ZAP.Payment.Application.Features.PaymentTypes.DTOs;

namespace ZAP.Payment.Application.Features.PaymentTypes.Queries
{
    public class GetPaymentTypeByIdQuery : IRequest<PaymentTypeDto>
    {
        public string Id { get; set; }

        public GetPaymentTypeByIdQuery(string id)
        {
            Id = id;
        }
    }
}
