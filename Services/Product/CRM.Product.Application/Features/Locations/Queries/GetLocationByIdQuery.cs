using MediatR;
using CRM.Product.Application.Features.Locations.DTOs;
using System;

namespace CRM.Product.Application.Features.Locations.Queries
{
    public class GetLocationByIdQuery : IRequest<LocationDto?>
    {
        public Guid Id { get; set; }
        public int LocaleId { get; set; } = 2; // Default to 2 (VI)
    }

}
