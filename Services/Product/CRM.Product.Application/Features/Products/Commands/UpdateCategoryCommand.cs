using MediatR;
using System;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid? parent_id { get; set; }
        public string? legacy_id { get; set; }
        public string? name { get; set; }
        public string? slug { get; set; }
        public string? icon_url { get; set; }
        public string? banner_url { get; set; }
        public int? sort_order { get; set; }
        public string? meta_title { get; set; }
        public string? meta_description { get; set; }
        public int? status_id { get; set; }
        public bool? is_active { get; set; }
        public string? seo_title { get; set; }
        public string? seo_description { get; set; }
    }
}
