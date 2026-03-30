using CRM.BuildingBlocks.Extensions;
using CRM.BuildingBlocks.Models;
using CRM.Order.Application.Orders.Queries.GetOrderList;
using CRM.Order.Application.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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

        /// <summary>Health check</summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "CRM Order API is running", Time = DateTime.UtcNow });
        }

        /// <summary>
        /// GET paginated order list.
        /// POST body: { "PageIndex": 1, "PageSize": 20, "Keyword": "", "Status": "" }
        /// </summary>
        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var filter = await Request.GetRawBodyAsync<FilterDTOs>();
            var result = await _mediator.Send(new GetOrderListQuery { Filter = filter });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

    }
}
