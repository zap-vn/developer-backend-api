using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.Product.Application.Features.Collections.DTOs;
using CRM.Product.Application.Features.Collections.Queries;
using System.Threading.Tasks;

namespace CRM.Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CollectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] CollectionListRequestDto request)
        {
            var query = new GetCollectionListQuery
            {
                PageIndex = request.page_index,
                PageSize = request.page_size,
                Search = request.search
            };

            var result = await _mediator.Send(query);
            return Ok(new { success = true, data = result });
        }
    }
}
