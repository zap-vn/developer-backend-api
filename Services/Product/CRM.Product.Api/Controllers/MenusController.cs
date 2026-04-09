using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Menus.DTOs;
using CRM.Product.Application.Features.Menus.Queries;
using CRM.Product.Application.Features.Menus.Commands;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/menus")]
    public class MenusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        [Consumes("application/json")]
        public async Task<IActionResult> List([FromBody] MenuListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetMenuListQuery { Request = requestBody });
            
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
        public async Task<ActionResult<PagedResult<MenuListResultDto>>> Get([FromQuery] MenuListRequestDto pagination)
        {
            var result = await _mediator.Send(new GetMenuListQuery { Request = pagination });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMenuCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuCommand command)
        {
            if (id != command.id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteMenuCommand { id = id });
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
