using MediatR;
using CRM.Product.Application.Features.Warehouses.DTOs;
using System;

namespace CRM.Product.Application.Features.Warehouses.Queries
{
    public class GetWarehouseByIdQuery : IRequest<WarehouseDto?>
    {
        public Guid Id { get; set; }
    }
}
