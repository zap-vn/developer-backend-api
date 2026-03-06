using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Payment.Domain.Interfaces;
using System;

namespace ZAP.Payment.Application.Features.PaymentTypes.Commands
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
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return false;
            var entity = await _repository.GetByIdAsync(parsedId);
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
