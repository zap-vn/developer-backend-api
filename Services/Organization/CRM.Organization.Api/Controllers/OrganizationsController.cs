using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Organization.Api.Controllers
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
            return Ok(new { Status = "CRM Organization API is running", Time = DateTime.UtcNow });
        }
    }
}
