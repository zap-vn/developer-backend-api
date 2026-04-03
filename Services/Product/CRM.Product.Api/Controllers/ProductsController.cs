using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using CRM.Product.Application.Features.Products.Commands;
using CRM.Product.Application.Features.Products.Queries;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using CRM.BuildingBlocks.Extensions;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM Product API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        [Consumes("application/json")]
        public async Task<IActionResult> List([FromBody] ProductListRequestDto requestBody)
        {
            Console.WriteLine(">>> LOG: ProductsController.List reached <<<");
            Console.WriteLine($">>> LOG: Page={requestBody.Page}, PageSize={requestBody.PageSize} <<<");

            var result = await _mediator.Send(new GetProductListQuery { Request = requestBody });
            
            return Ok(new 
            {
                success = true,
                code = 200,
                message = "OK",
                data = new 
                {
                    total_page = (int)Math.Ceiling((double)result.TotalCount / result.PageSize),
                    total_record = result.TotalCount,
                    page_index = result.CurrentPage,
                    page_size = result.PageSize,
                    items = result.Items.Select(x => new
                    {
                        id = x.id,
                        cate_name = x.category_name ?? "TBD",
                        name = x.variant_name ?? x.name,
                        price = x.sale_price ?? 0,
                        status = x.status_code ?? (x.status_id?.ToString() ?? "129")
                    }).ToList()
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id; 
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
