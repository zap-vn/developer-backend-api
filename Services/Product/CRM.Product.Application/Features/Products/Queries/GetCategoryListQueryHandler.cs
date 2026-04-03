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
    public class GetCategoryListQueryHandler : IRequestHandler<GetCategoryListQuery, PagedResult<CategoryDto>>
    {
        private readonly ICategoryRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetCategoryListQueryHandler(ICategoryRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<CategoryDto>> Handle(GetCategoryListQuery request, CancellationToken cancellationToken)
        {
            var tenantIdString = _currentUserService.UserGuid;
            Guid? tenantId = null;
            if (Guid.TryParse(tenantIdString, out var guid)) tenantId = guid;

            var (items, total) = await _repository.GetPagedAsync(
                request.Request.PageIndex, 
                request.Request.PageSize, 
                tenantId, 
                request.Request.SearchTerm);

            var dtos = items.Select(x => new CategoryDto
            {
                id = x.id,
                parent_id = x.parent_id,
                legacy_id = x.legacy_id,
                materialized_path = x.materialized_path,
                name = x.name,
                slug = x.slug,
                icon_url = x.icon_url,
                banner_url = x.banner_url,
                sort_order = x.sort_order ?? 0,
                meta_title = x.meta_title,
                meta_description = x.meta_description,
                status_id = x.status_id,
                status_text = x.status?.code,
                is_active = x.is_active,
                seo_title = x.seo_title,
                seo_description = x.seo_description
            }).ToList();

            return new PagedResult<CategoryDto>(dtos, total, request.Request.PageIndex, request.Request.PageSize);
        }
    }
}
