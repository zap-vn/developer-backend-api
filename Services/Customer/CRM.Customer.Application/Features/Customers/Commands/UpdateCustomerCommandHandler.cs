using MediatR;
using CRM.Customer.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Customer.Application.Features.Customers.Commands
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
    {
        private readonly ICustomerRepository _repository;

        public UpdateCustomerCommandHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            entity.BusinessName = request.BusinessName;
            entity.MerchantName = request.MerchantName;
            entity.Email = request.Email;
            entity.CustomerCode = request.CustomerCode;
            entity.Name = request.Name;
            entity.PhoneNumber = request.PhoneNumber;
            entity.Address = request.Address;
            entity.IsActive = request.IsActive;

            return await _repository.UpdateAsync(entity);
        }
    }
}
