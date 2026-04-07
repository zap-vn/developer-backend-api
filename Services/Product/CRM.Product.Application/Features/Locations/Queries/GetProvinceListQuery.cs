using MediatR;
using System.Collections.Generic;
using CRM.Product.Application.Features.Locations.DTOs;

namespace CRM.Product.Application.Features.Locations.Queries
{
    public class GetProvinceListQuery : IRequest<List<ProvinceDto>>
    {
        public int LocaleId { get; set; } = 1;
    }
}
