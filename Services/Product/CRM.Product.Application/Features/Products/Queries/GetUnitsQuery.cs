using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using MediatR;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetUnitsQuery : IRequest<PagedResult<UnitDto>>
    {
        public UnitListRequestDto Request { get; set; } = new();
    }
}
