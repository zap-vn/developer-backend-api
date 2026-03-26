using MediatR;
using CRM.Product.Application.Features.Products.DTOs;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Domain.Interfaces;
using System;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new ProductDto 
            { 
#pragma warning disable CS8602
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                Status = entity.Visible,
                CateName = entity.Category,
                MerchantId = entity.UserGuid
            };
        }
    }
}
