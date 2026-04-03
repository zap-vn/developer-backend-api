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
    [Route("api/brands")]
    public class BrandsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrandsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] BrandListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetBrandsQuery { Request = requestBody });
            
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
        public async Task<ActionResult<PagedResult<BrandDto>>> Get([FromQuery] BrandListRequestDto pagination)
        {
            var result = await _mediator.Send(new GetBrandsQuery { Request = pagination });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateBrandCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteBrandCommand { Id = id });
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
