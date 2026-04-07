using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Product.Application.Features.Locations.Queries;
using CRM.Product.Application.Features.Locations.DTOs;
using CRM.Product.Application.Features.Locations.Commands;
using CRM.BuildingBlocks.Models;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/Locations")]
    public class LocationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        [Consumes("application/json")]
        public async Task<IActionResult> List([FromBody] LocationListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetLocationListQuery { Request = requestBody });
            
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetLocationByIdQuery { Id = id });
            if (result == null)
                return NotFound(new { success = false, code = 404, message = "Location not found" });

            return Ok(new { success = true, code = 200, message = "OK", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationCommand command)
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

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces([FromQuery] int locale_id = 1)
        {
            var result = await _mediator.Send(new GetProvinceListQuery { LocaleId = locale_id });
            return Ok(new 
            {
                success = true,
                code = 200,
                message = "OK",
                data = result
            });
        }
    }
}
