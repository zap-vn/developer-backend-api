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
                Id = x.id,
                ParentId = x.parent_id,
                Name = x.name,
                IsActive = true,
                IconUrl = x.icon_url,
                MaterializedPath = x.materialized_path,
                SeoTitle = x.seo_title,
                SeoDescription = x.seo_description
            }).ToList();

            return new PagedResult<CategoryDto>(dtos, total, request.Request.PageIndex, request.Request.PageSize);
        }
    }
}
