using MediatR;
using ZAP.Payment.Application.Features.PaymentTerms.DTOs;

namespace ZAP.Payment.Application.Features.PaymentTerms.Queries
{
    public class GetPaymentTermsByIdQuery : IRequest<PaymentTermsDto>
    {
        public string Id { get; set; }

        public GetPaymentTermsByIdQuery(string id)
        {
            Id = id;
        }
    }
}
