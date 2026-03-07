using MediatR;
using CRM.Payment.Application.Features.PaymentTerms.DTOs;

namespace CRM.Payment.Application.Features.PaymentTerms.Queries
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
