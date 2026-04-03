using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.BuildingBlocks.Models;
using CRM.Order.Application.Features.Orders.Queries;
using CRM.Order.Application.Features.Orders.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRM.Order.Api.Controllers
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

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] GetTransactionManagementListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IReadOnlyList<TransactionListDto>>.SuccessResult(
                result.Items, 
                new PaginationMetadata(result.CurrentPage, result.PageSize, result.TotalCount)));
        }
    }
}
