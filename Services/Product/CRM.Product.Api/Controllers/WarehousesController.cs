using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Product.Application.Features.Warehouses.Queries;
using CRM.Product.Application.Features.Warehouses.DTOs;
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
                    total_page = (int)System.Math.Ceiling((double)result.TotalCount / result.PageSize),
                    total_record = result.TotalCount,
                    page_index = result.CurrentPage,
                    page_size = result.PageSize,
                    items = result.Items
                }
            });
        }
    }
}
