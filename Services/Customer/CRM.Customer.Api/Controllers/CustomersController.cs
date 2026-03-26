using CRM.Customer.Application.Features.Customers.Commands;
using CRM.Customer.Application.Features.Customers.Queries;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.BuildingBlocks.Models;
using CRM.BuildingBlocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CRM.Customer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM Customer API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var filter = await Request.GetRawBodyAsync<FilterDTOs>();
            var result = await _mediator.Send(new GetCustomerListQuery { Filter = filter });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerCommand command)
        {
            command.Id = id; 
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
