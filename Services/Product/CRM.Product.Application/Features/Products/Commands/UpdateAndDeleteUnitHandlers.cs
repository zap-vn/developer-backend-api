using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, bool>
    {
        private readonly IUnitRepository _repository;

        public UpdateUnitCommandHandler(IUnitRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            if (request.Code != null) entity.code = request.Code;
            if (request.Name != null) entity.name = request.Name;
            if (request.UomType != null) entity.uom_type = request.UomType;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }

    public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, bool>
    {
        private readonly IUnitRepository _repository;

        public DeleteUnitCommandHandler(IUnitRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
