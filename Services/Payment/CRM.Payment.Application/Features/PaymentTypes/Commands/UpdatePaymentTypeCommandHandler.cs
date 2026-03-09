using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Payment.Domain.Interfaces;
using System;

namespace CRM.Payment.Application.Features.PaymentTypes.Commands
{
    public class UpdatePaymentTypeCommandHandler : IRequestHandler<UpdatePaymentTypeCommand, bool>
    {
        private readonly IPaymentTypeRepository _repository;

        public UpdatePaymentTypeCommandHandler(IPaymentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePaymentTypeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return false;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
