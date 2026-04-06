using MediatR;
using System;

namespace CRM.Product.Application.Features.Warehouses.Commands
{
    public class CreateWarehouseCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public string? Description { get; set; }
        public string? WarehouseType { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? XLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? FacebookLink { get; set; }
        public string? LogoUrl { get; set; }
        public string? BrandColor { get; set; }
        public string? Timezone { get; set; }
        public string? BusinessHours { get; set; }
        public string? PreferredLanguage { get; set; }
        public Guid? MatchLocationId { get; set; }
    }
}
