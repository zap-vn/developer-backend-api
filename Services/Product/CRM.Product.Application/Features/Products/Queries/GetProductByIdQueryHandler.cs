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
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return null;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return null;

            return new ProductDto 
            { 
#pragma warning disable CS8602
                Id = entity.Id.ToString(),
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock,
                Category = entity.Category,
                ImageUrl = entity.ImageUrl
#pragma warning restore CS8602
            };
        }
    }
}
