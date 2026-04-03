using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.BuildingBlocks.Models;
using CRM.Promotion.Application.Features.Promotions.Queries;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRM.Promotion.Api.Controllers
{
    [ApiController]
    [Route("api/v1/management")]
    public class ManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("promotions")]
        public async Task<IActionResult> GetPromotions([FromQuery] GetPromotionListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IReadOnlyList<PromotionListDto>>.SuccessResult(
                result.Items, 
                new PaginationMetadata(result.CurrentPage, result.PageSize, result.TotalCount)));
        }
    }
}
