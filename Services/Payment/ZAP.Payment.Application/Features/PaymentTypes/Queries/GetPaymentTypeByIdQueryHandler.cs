using MediatR;
using ZAP.Payment.Application.Features.PaymentTypes.DTOs;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Payment.Domain.Interfaces;
using System;

namespace ZAP.Payment.Application.Features.PaymentTypes.Queries
{
    public class GetPaymentTypeByIdQueryHandler : IRequestHandler<GetPaymentTypeByIdQuery, PaymentTypeDto>
    {
        private readonly IPaymentTypeRepository _repository;

        public GetPaymentTypeByIdQueryHandler(IPaymentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaymentTypeDto> Handle(GetPaymentTypeByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return null;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return null;

            return new PaymentTypeDto 
            { 
#pragma warning disable CS8602
                Id = entity.Id.ToString(),
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
#pragma warning restore CS8602
            };
        }
    }
}
