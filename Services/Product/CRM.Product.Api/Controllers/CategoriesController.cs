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
        [Consumes("application/json")]
        public async Task<IActionResult> List([FromBody] CategoryListRequestDto requestBody)
        {
            var result = await _mediator.Send(new GetCategoryListQuery { Request = requestBody });
            
            return Ok(new 
            {
                success = true,
                data = new 
                {
                    items = result.Items,
                    total = result.TotalCount,
                    total_page = result.PageSize > 0 ? (int)System.Math.Ceiling(result.TotalCount / (double)result.PageSize) : 1,
                    total_record = result.TotalCount,
                    page_index = result.CurrentPage,
                    page_size = result.PageSize
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
