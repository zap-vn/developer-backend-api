using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Customer.Domain.Interfaces;
using System;

namespace CRM.Customer.Application.Features.CustomerGroups.Commands
{
    public class UpdateCustomerGroupCommandHandler : IRequestHandler<UpdateCustomerGroupCommand, bool>
    {
        private readonly ICustomerGroupRepository _repository;

        public UpdateCustomerGroupCommandHandler(ICustomerGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateCustomerGroupCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return false;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.DiscountPercentage = request.DiscountPercentage;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
