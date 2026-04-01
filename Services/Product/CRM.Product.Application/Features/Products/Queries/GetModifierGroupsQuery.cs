using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using MediatR;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetModifierGroupsQuery : IRequest<PagedResult<ModifierGroupDto>>
    {
        public ModifierGroupListRequestDto Request { get; set; } = new();
    }
}
