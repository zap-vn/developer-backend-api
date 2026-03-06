using MediatR;
using ZAP.Product.Application.Features.Products.DTOs;

namespace ZAP.Product.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public string Id { get; set; }

        public GetProductByIdQuery(string id)
        {
            Id = id;
        }
    }
}
