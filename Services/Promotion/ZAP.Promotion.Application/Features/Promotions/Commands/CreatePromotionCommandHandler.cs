using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Promotion.Domain.Entities;
using ZAP.Promotion.Domain.Interfaces;

namespace ZAP.Promotion.Application.Features.Promotions.Commands
{
    public class CreatePromotionCommandHandler : IRequestHandler<CreatePromotionCommand, string>
    {
        private readonly IPromotionRepository _repository;

        public CreatePromotionCommandHandler(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
        {
            var entity = new PromotionEntity
            {
                Code = request.Code,
                Title = request.Title,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                DiscountValue = request.DiscountValue,
                DiscountType = request.DiscountType
            };

            await _repository.CreateAsync(entity);
            return entity.Id.ToString();
        }
    }
}
