using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Payment.Domain.Entities;
using CRM.Payment.Domain.Interfaces;

namespace CRM.Payment.Application.Features.PaymentTerms.Commands
{
    public class CreatePaymentTermsCommandHandler : IRequestHandler<CreatePaymentTermsCommand, string>
    {
        private readonly IPaymentTermsRepository _repository;

        public CreatePaymentTermsCommandHandler(IPaymentTermsRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(CreatePaymentTermsCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Entities.PaymentTerms
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Days = request.Days,
                IsActive = request.IsActive
            };

            await _repository.CreateAsync(entity);
            return entity.Id.ToString();
        }
    }
}
