using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Order.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM Order API is running", Time = DateTime.UtcNow });
        }
    }
}
