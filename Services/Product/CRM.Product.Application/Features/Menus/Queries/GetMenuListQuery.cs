using MediatR;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Menus.DTOs;
using System;

namespace CRM.Product.Application.Features.Menus.Queries
{
    public class GetMenuListQuery : IRequest<PagedResult<MenuListResultDto>>
    {
        public Guid? TenantId { get; set; }
        public string? Name { get; set; }
        public string? MenuType { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
