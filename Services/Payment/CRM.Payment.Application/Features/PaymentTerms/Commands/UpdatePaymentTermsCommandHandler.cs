using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Payment.Domain.Interfaces;
using System;

namespace CRM.Payment.Application.Features.PaymentTerms.Commands
{
    public class UpdatePaymentTermsCommandHandler : IRequestHandler<UpdatePaymentTermsCommand, bool>
    {
        private readonly IPaymentTermsRepository _repository;

        public UpdatePaymentTermsCommandHandler(IPaymentTermsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePaymentTermsCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return false;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return false;

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Days = request.Days;
            entity.IsActive = request.IsActive;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
