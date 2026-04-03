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
            return Ok(ApiResponse<IReadOnlyList<MembershipListDto>>.SuccessResult(
                result.Items, 
                new PaginationMetadata(result.CurrentPage, result.PageSize, result.TotalCount)));
        }
    }
}
