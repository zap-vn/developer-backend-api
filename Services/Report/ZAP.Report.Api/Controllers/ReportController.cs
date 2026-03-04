using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using ZAP.Report.Application.Reports.DTOs;
using ZAP.Report.Application.Reports.Queries.GetOverviewListLocation;
using Microsoft.AspNetCore.Authorization;

namespace ZAP.Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    // [Authorize] // Uncomment when authentication is configured
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("overview-list-location")]
        public async Task<IActionResult> GetOverviewListLocation([FromBody] ReportRequestDto request)
        {
            // var userGuid = User.FindFirst("UserGuid")?.Value ?? "123456"; 
            // Mocking UserGuid for now, should extract from JWT Token.
            var userGuid = "123456";

            var result = await _mediator.Send(new GetOverviewListLocationQuery 
            { 
                Request = request,
                UserGuid = userGuid
            });

            return Ok(new 
            {
                Code = 200,
                Message = "Thành công",
                DidError = false,
                Model = new { Overview = result }
            });
        }
    }
}
