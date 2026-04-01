using System;
using MediatR;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateBrandCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public int? StatusId { get; set; }
        public bool? IsPremium { get; set; }
    }

    public class DeleteBrandCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
