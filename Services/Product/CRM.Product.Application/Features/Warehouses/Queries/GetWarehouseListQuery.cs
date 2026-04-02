using MediatR;
using CRM.Product.Application.Features.Warehouses.DTOs;
using CRM.BuildingBlocks.Models;
using System.Collections.Generic;
using System;

namespace CRM.Product.Application.Features.Warehouses.Queries
{
    public class GetWarehouseListQuery : IRequest<PagedResult<WarehouseDto>>
    {
        public WarehouseListRequestDto Request { get; set; } = new WarehouseListRequestDto();
    }
}
