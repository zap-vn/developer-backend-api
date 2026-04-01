using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] CategoryListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetCategoryListQuery { Request = requestBody });
            
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

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());
            return Ok(result);
        }
    }
}
