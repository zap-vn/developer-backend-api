using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Product.Application.Features.DiningOptions.Queries;
using CRM.Product.Application.Features.DiningOptions.Commands;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/diningoptions")]
    public class DiningOptionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiningOptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetDiningOptionListQuery());
            return Ok(new { success = true, code = 200, message = "OK", data = result });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetDiningOptionByIdQuery { Id = id });
            if (result == null)
                return NotFound(new { success = false, code = 404, message = "Dining option not found", data = (object?)null });

            return Ok(new { success = true, code = 200, message = "OK", data = result });
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Create([FromBody] CreateDiningOptionCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { success = true, code = 200, message = "Created successfully", data = new { id } });
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiningOptionCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { success = false, code = 404, message = "Dining option not found", data = (object?)null });

            return Ok(new { success = true, code = 200, message = "Updated successfully", data = new { id } });
        }
    }
}
