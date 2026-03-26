using CRM.HR.Application.Employees.Commands;
using CRM.HR.Application.Employees.Commands.CreateEmployee;
using CRM.HR.Application.Employees.Queries;
using CRM.BuildingBlocks.Models;
using CRM.BuildingBlocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace CRM.HR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM HR API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var filter = await Request.GetRawBodyAsync<FilterDTOs>();
            var result = await _mediator.Send(new GetEmployeeListQuery { Filter = filter });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateEmployeeCommand command)
        {
            command.Id = id; 
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
