using MediatR;
using CRM.Product.Application.Features.Locations.DTOs;
using System;

namespace CRM.Product.Application.Features.Locations.Queries
{
    public class GetLocationByIdQuery : IRequest<LocationDto?>
    {
        public Guid Id { get; set; }
    }
}
