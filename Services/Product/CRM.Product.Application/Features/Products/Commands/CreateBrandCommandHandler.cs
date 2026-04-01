using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Guid>
    {
        private readonly IBrandRepository _repository;

        public CreateBrandCommandHandler(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var entity = new Brand
            {
                tenant_id = request.TenantId,
                name = request.Name,
                slug = request.Slug,
                logo_url = request.LogoUrl,
                banner_url = request.BannerUrl,
                website_url = request.WebsiteUrl,
                status_id = request.StatusId,
                is_premium = request.IsPremium
            };

            await _repository.CreateAsync(entity);
            return entity.id;
        }
    }
}
