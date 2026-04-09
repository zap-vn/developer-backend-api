using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Payment.Application.Features.PaymentTerms.Commands;
using CRM.Payment.Application.Features.PaymentTerms.Queries;
using CRM.Payment.Application.Features.PaymentTerms.DTOs;
using CRM.BuildingBlocks.Models;
using CRM.BuildingBlocks.Extensions;

namespace CRM.Payment.Api.Controllers
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
            return Ok(new { Status = "CRM PaymentTerms API is running", Time = System.DateTime.UtcNow });
        }

        [HttpPost("list")]
        [Consumes("application/json")]
        public async Task<IActionResult> List([FromBody] FilterDTOs filter)
        {
            var result = await _mediator.Send(new GetPaymentTermsListQuery { Filter = filter });
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetPaymentTermsByIdQuery(id));
            if (result == null)
                return NotFound(new { success = false, code = 404, message = "PaymentTerms not found", data = (object?)null });

            return Ok(new { success = true, code = 200, message = "OK", data = result });
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentTermsCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { success = true, code = 200, message = "Created successfully", data = new { id } });
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePaymentTermsCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { success = false, code = 404, message = "PaymentTerms not found", data = (object?)null });

            return Ok(new { success = true, code = 200, message = "Updated successfully", data = new { id } });
        }
    }
}
