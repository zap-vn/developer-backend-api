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
    [Route("api/modifiergroups")]
    public class ModifierGroupsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ModifierGroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] ModifierGroupListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetModifierGroupsQuery { Request = requestBody });
            
            return Ok(new 
            {
                success = true,
                data = new 
                {
                    items = result.Items,
                    total = result.TotalCount
                }
            });
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ModifierGroupDto>>> Get([FromQuery] ModifierGroupListRequestDto pagination)
        {
            var result = await _mediator.Send(new GetModifierGroupsQuery { Request = pagination });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateModifierGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateModifierGroupCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteModifierGroupCommand { Id = id });
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
