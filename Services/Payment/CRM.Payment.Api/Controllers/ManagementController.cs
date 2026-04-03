using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.BuildingBlocks.Models;
using CRM.Payment.Application.Features.Payments.Queries;
using CRM.Payment.Application.Features.Payments.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRM.Payment.Api.Controllers
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

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments([FromQuery] GetPaymentListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IReadOnlyList<PaymentListDto>>.SuccessResult(
                result.Items, 
                new PaginationMetadata(result.CurrentPage, result.PageSize, result.TotalCount)));
        }
    }
}
