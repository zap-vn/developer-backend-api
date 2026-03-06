using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ZAP.Organization.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrganizationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "ZAP Organization API is running", Time = DateTime.UtcNow });
        }
    }
}
