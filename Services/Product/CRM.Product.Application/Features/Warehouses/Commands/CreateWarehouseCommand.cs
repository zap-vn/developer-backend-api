using MediatR;
using System;

namespace CRM.Product.Application.Features.Warehouses.Commands
{
    public class CreateWarehouseCommand : IRequest<Guid>
    {
        public Guid? tenant_id { get; set; }
        public Guid? node_id { get; set; }
        public string? legacy_id { get; set; }
        public string name { get; set; } = string.Empty;
        public int? status_id { get; set; }
        public bool? is_active { get; set; } = true;
        public string? slug { get; set; }
        public string? business_name { get; set; }
        public string? description { get; set; }
        public int? location_type_id { get; set; }
        public string? address_line_1 { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public int? country_id { get; set; }
        public int? province_id { get; set; }
        public int? district_id { get; set; }
        public int? ward_id { get; set; }
        public string? zipcode { get; set; }
        public string? phone_number { get; set; }
        public string? email { get; set; }
        public string? website { get; set; }
        public string? twitter { get; set; }
        public string? instagram { get; set; }
        public string? facebook { get; set; }
        public string? logo_url { get; set; }
        public string? cover_image_url { get; set; }
        public string? brand_color { get; set; }
        public string? timezone { get; set; }
        public System.Text.Json.JsonElement? operating_hours { get; set; }
        public string? transfer_account { get; set; }
        public string? transfer_tag { get; set; }
        public Guid? parent_location_id { get; set; }
    }
}
