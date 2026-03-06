using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Promotion.Domain.Interfaces;

namespace ZAP.Promotion.Application.Features.Promotions.Commands
{
    public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, bool>
    {
        private readonly IPromotionRepository _repository;

        public UpdatePromotionCommandHandler(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return false;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return false;

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.DiscountValue = request.DiscountValue;
            entity.DiscountType = request.DiscountType;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
