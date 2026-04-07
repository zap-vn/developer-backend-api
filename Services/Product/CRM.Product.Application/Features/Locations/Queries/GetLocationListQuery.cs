using MediatR;
using CRM.Product.Application.Features.Locations.DTOs;
using CRM.BuildingBlocks.Models;
using System.Collections.Generic;
using System;

namespace CRM.Product.Application.Features.Locations.Queries
{
    public class GetLocationListQuery : IRequest<PagedResult<LocationDto>>
    {
        public LocationListRequestDto Request { get; set; } = new LocationListRequestDto();
    }
}
