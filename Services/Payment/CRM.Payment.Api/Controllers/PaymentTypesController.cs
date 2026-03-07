using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Payment.Application.Features.PaymentTypes.Commands;
using CRM.Payment.Application.Features.PaymentTypes.Queries;
using CRM.Payment.Application.Features.PaymentTypes.DTOs;

namespace CRM.Payment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM PaymentTypes API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GetPaymentTypeListQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetPaymentTypeByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePaymentTypeCommand command)
        {
            command.Id = id; 
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
