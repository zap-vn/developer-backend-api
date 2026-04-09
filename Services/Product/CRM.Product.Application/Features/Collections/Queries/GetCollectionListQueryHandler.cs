using MediatR;
using CRM.Product.Application.Features.Collections.DTOs;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Domain.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRM.Product.Application.Features.Collections.Queries
{
    public class GetCollectionListQueryHandler : IRequestHandler<GetCollectionListQuery, PagedResult<CollectionDto>>
    {
        private readonly ICollectionRepository _repository;

        public GetCollectionListQueryHandler(ICollectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<CollectionDto>> Handle(GetCollectionListQuery request, CancellationToken cancellationToken)
        {
            var (items, total) = await _repository.GetPagedAsync(request.PageIndex, request.PageSize, request.Search);

            var dtos = items.Select(c => new CollectionDto
            {
                id = c.id,
                name = c.name,
                slug = c.slug,
                description_html = c.description_html,
                banner_url = c.banner_url,
                status_id = c.status_id,
                sort_order = c.sort_order
            }).ToList();

            return new PagedResult<CollectionDto>(dtos, total, request.PageIndex, request.PageSize);
        }
    }
}
