using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, PagedResult<BrandDto>>
    {
        private readonly IBrandRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetBrandsQueryHandler(IBrandRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<BrandDto>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            var tenantIdString = _currentUserService.UserGuid;
            Guid? tenantId = null;
            if (Guid.TryParse(tenantIdString, out var guid)) tenantId = guid;

            var (items, total) = await _repository.GetPagedAsync(
                request.Request.PageIndex, 
                request.Request.PageSize, 
                tenantId, 
                request.Request.SearchTerm);

            var dtos = items.Select(x => new BrandDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                name = x.name,
                slug = x.slug,
                logo_url = x.logo_url,
                banner_url = x.banner_url,
                website_url = x.website_url,
                status_id = x.status_id,
                status_text = x.status?.code,
                is_premium = x.is_premium
            });

            return new PagedResult<BrandDto>(dtos.ToList(), total, request.Request.PageIndex, request.Request.PageSize);
        }
    }
}
