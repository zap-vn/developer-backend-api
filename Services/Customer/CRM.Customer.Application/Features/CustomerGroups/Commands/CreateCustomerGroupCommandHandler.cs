using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Customer.Domain.Entities;
using CRM.Customer.Domain.Interfaces;

namespace CRM.Customer.Application.Features.CustomerGroups.Commands
{
    public class CreateCustomerGroupCommandHandler : IRequestHandler<CreateCustomerGroupCommand, string>
    {
        private readonly ICustomerGroupRepository _repository;

        public CreateCustomerGroupCommandHandler(ICustomerGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(CreateCustomerGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = new CustomerGroup
            {
                Name = request.Name,
                Description = request.Description,
                DiscountPercentage = request.DiscountPercentage
            };

            await _repository.CreateAsync(entity);
            return entity.Id.ToString();
        }
    }
}
