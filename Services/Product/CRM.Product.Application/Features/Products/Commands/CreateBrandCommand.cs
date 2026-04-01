using System;
using MediatR;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateBrandCommand : IRequest<Guid>
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string BannerUrl { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public int StatusId { get; set; } = 2101;
        public bool IsPremium { get; set; }
    }
}
