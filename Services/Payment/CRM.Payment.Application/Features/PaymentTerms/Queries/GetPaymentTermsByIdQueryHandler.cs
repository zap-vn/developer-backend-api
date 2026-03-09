using MediatR;
using CRM.Payment.Application.Features.PaymentTerms.DTOs;
using System.Threading;
using System.Threading.Tasks;
using CRM.Payment.Domain.Interfaces;
using System;

namespace CRM.Payment.Application.Features.PaymentTerms.Queries
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
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new PaymentTermsDto 
            { 
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                Days = entity.Days,
                IsActive = entity.IsActive
            };
        }
    }
}
