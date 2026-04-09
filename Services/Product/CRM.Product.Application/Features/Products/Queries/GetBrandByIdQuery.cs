using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetBrandByIdQuery : IRequest<BrandDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandDto?>
    {
        private readonly IBrandRepository _repository;

        public GetBrandByIdQueryHandler(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<BrandDto?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var x = await _repository.GetByIdAsync(request.Id);
            if (x == null) return null;

            return new BrandDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                name = x.name,
                slug = x.slug,
                logo_url = x.logo_url,
                banner_url = x.banner_url,
                website_url = x.website_url,
                status_id = x.status_id,
                is_premium = x.is_premium
            };
        }
    }
}
