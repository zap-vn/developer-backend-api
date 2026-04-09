using MediatR;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Menus.DTOs;
using System;

namespace CRM.Product.Application.Features.Menus.Queries
{
    public class GetMenuListQuery : IRequest<PagedResult<MenuListResultDto>>
    {
        public Guid? TenantId { get; set; }
        public MenuListRequestDto Request { get; set; } = new();
    }
}
