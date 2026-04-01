using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, bool>
    {
        private readonly IBrandRepository _repository;

        public UpdateBrandCommandHandler(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            if (request.Name != null) entity.name = request.Name;
            if (request.Slug != null) entity.slug = request.Slug;
            if (request.LogoUrl != null) entity.logo_url = request.LogoUrl;
            if (request.BannerUrl != null) entity.banner_url = request.BannerUrl;
            if (request.WebsiteUrl != null) entity.website_url = request.WebsiteUrl;
            if (request.StatusId.HasValue) entity.status_id = request.StatusId.Value;
            if (request.IsPremium.HasValue) entity.is_premium = request.IsPremium.Value;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }

    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, bool>
    {
        private readonly IBrandRepository _repository;

        public DeleteBrandCommandHandler(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
