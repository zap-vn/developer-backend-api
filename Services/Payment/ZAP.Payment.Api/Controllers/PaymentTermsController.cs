using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZAP.Payment.Application.Features.PaymentTerms.Commands;
using ZAP.Payment.Application.Features.PaymentTerms.Queries;
using ZAP.Payment.Application.Features.PaymentTerms.DTOs;

namespace ZAP.Payment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTermsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentTermsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "ZAP PaymentTerms API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GetPaymentTermsListQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetPaymentTermsByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentTermsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePaymentTermsCommand command)
        {
            command.Id = id; 
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
