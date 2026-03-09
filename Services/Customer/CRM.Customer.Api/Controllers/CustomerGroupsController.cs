using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Customer.Application.Features.CustomerGroups.Commands;
using CRM.Customer.Application.Features.CustomerGroups.Queries;
using CRM.Customer.Application.Features.CustomerGroups.DTOs;
using CRM.BuildingBlocks.Models;
using CRM.BuildingBlocks.Extensions;

namespace CRM.Customer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerGroupsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerGroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM Customer (Groups) API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var filter = await Request.GetRawBodyAsync<FilterDTOs>();
            var result = await _mediator.Send(new GetCustomerGroupListQuery { Filter = filter });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetCustomerGroupByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerGroupCommand command)
        {
            command.Id = id; 
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
