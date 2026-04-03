using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.BuildingBlocks.Models;
using CRM.Customer.Application.Features.Customers.Queries;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.Customer.Application.Features.Memberships.Queries;
using CRM.Customer.Application.Features.Memberships.DTOs;
using System.Threading.Tasks;
using System;

namespace CRM.Customer.Api.Controllers
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

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers([FromQuery] GetCustomerManagementListQuery query)
        {
            // Use TenantId from current context if mandatory
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IReadOnlyList<CustomerListDto>>.SuccessResult(
                result.Items, 
                new PaginationMetadata(result.CurrentPage, result.PageSize, result.TotalCount)));
        }

        [HttpGet("memberships")]
        public async Task<IActionResult> GetMemberships([FromQuery] GetMembershipListQuery query)
        {
            var result = await _mediator.Send(query);
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
    }
}
