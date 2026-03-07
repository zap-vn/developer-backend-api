using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Payment.Domain.Entities;
using CRM.Payment.Domain.Interfaces;

namespace CRM.Payment.Application.Features.PaymentTypes.Commands
{
    public class CreatePaymentTypeCommandHandler : IRequestHandler<CreatePaymentTypeCommand, string>
    {
        private readonly IPaymentTypeRepository _repository;

        public CreatePaymentTypeCommandHandler(IPaymentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(CreatePaymentTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new PaymentType
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _repository.CreateAsync(entity);
            return entity.Id.ToString();
        }
    }
}
