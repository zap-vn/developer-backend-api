using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class DeleteModifierGroupCommandHandler : IRequestHandler<DeleteModifierGroupCommand, bool>
    {
        private readonly IModifierGroupRepository _repository;

        public DeleteModifierGroupCommandHandler(IModifierGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteModifierGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
