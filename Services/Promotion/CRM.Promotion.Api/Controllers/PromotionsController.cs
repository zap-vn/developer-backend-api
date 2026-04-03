using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Promotion.Application.Features.Promotions.Commands;
using CRM.Promotion.Application.Features.Promotions.Queries;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.BuildingBlocks.Models;
using CRM.BuildingBlocks.Extensions;

namespace CRM.Promotion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromotionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM Promotion API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var filter = await Request.GetRawBodyAsync<FilterDTOs>();
            var result = await _mediator.Send(new GetPromotionListQuery { Filter = filter });
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
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetPromotionByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromotionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePromotionCommand command)
        {
            command.Id = id; 
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
