using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Product.Application.Features.Warehouses.Queries;
using CRM.Product.Application.Features.Warehouses.DTOs;
using CRM.Product.Application.Features.Warehouses.Commands;
using CRM.BuildingBlocks.Models;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehousesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WarehousesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        [Consumes("application/json")]
        public async Task<IActionResult> List([FromBody] WarehouseListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetWarehouseListQuery { Request = requestBody });
            
            return Ok(new 
            {
                success = true,
                code = 200,
                message = "OK",
                data = new 
                {
                    total_page = result.PageSize > 0 ? (int)System.Math.Ceiling((double)result.TotalCount / result.PageSize) : 1,
                    total_record = result.TotalCount,
                    page_index = result.CurrentPage,
                    page_size = result.PageSize,
                    items = result.Items
                }
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new 
            {
                success = true,
                code = 200,
                message = "Location created successfully",
                data = result
            });
        }
    }
}
