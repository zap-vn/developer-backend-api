using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Menus.Queries;
using CRM.Product.Application.Features.Menus.DTOs;
using CRM.Product.Application.Features.Prices.Queries;
using CRM.Product.Application.Features.Prices.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRM.Product.Api.Controllers
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

        [HttpGet("menus")]
        public async Task<IActionResult> GetMenus([FromQuery] GetMenuListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IReadOnlyList<MenuListResultDto>>.SuccessResult(
                result.Items, 
                new PaginationMetadata(result.CurrentPage, result.PageSize, result.TotalCount)));
        }

        [HttpGet("prices")]
        public async Task<IActionResult> GetPrices([FromQuery] GetPriceListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IReadOnlyList<PriceListDto>>.SuccessResult(
                result.Items, 
                new PaginationMetadata(result.CurrentPage, result.PageSize, result.TotalCount)));
        }
    }
}
