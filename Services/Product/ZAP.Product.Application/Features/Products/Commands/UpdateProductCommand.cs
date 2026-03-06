using MediatR;
using System;

namespace ZAP.Product.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty; // Injected from route
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
