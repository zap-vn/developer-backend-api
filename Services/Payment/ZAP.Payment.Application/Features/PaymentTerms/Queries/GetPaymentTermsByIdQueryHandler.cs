using MediatR;
using ZAP.Payment.Application.Features.PaymentTerms.DTOs;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Payment.Domain.Interfaces;
using System;

namespace ZAP.Payment.Application.Features.PaymentTerms.Queries
{
    public class GetPaymentTermsByIdQueryHandler : IRequestHandler<GetPaymentTermsByIdQuery, PaymentTermsDto>
    {
        private readonly IPaymentTermsRepository _repository;

        public GetPaymentTermsByIdQueryHandler(IPaymentTermsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaymentTermsDto> Handle(GetPaymentTermsByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return null;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return null;

            return new PaymentTermsDto 
            { 
#pragma warning disable CS8602
                Id = entity.Id.ToString(),
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                Days = entity.Days,
                IsActive = entity.IsActive
#pragma warning restore CS8602
            };
        }
    }
}
