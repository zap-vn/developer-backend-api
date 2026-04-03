using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Application.Features.Products.Queries;
using CRM.Product.Application.Features.Products.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Models;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/units")]
    public class UnitsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UnitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        [Consumes("application/json")]
        public async Task<IActionResult> List([FromBody] UnitListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetUnitsQuery { Request = requestBody });
            
            return Ok(new 
            {
                success = true,
                data = new 
                {
                    items = result.Items,
                    total = result.TotalCount,
                    total_page = result.PageSize > 0 ? (int)System.Math.Ceiling((double)result.TotalCount / result.PageSize) : 1,
                    total_record = result.TotalCount,
                    page_index = result.CurrentPage,
                    page_size = result.PageSize
                }
            });
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<UnitDto>>> Get([FromQuery] UnitListRequestDto pagination)
        {
            var result = await _mediator.Send(new GetUnitsQuery { Request = pagination });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUnitCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteUnitCommand { Id = id });
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
