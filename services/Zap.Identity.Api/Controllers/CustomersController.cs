using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using NSwag.Annotations;



namespace Zap.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CustomerDto>>> GetAll([FromQuery] int limit = 10, [FromQuery] int skip = 0, [FromQuery] bool sortDescending = true)
    {
        var filter = new FilterDto 
        { 
            Limit = limit, 
            Skip = skip,
            Sort = new List<SortItemDto> { new SortItemDto { SortKey = "CreateDate", SortMode = sortDescending ? -1 : 1 } }
        };
        var result = await _customerService.SearchAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Tìm kiếm khách hàng theo FilterDto (POST body)
    /// </summary>
   


    [HttpPost("list")]
    [OpenApiOperation("list", "")]
    [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CustomerDto>>> Search([FromBody] FilterDto? filter)
    {
        filter ??= new FilterDto { Limit = 10, Skip = 0 };
        
        // Trường hợp không có truyền fiter order by thì Sắp xếp theo ngày create
        if (filter.Sort == null || !filter.Sort.Any())
        {
            // Kiểm tra legacy SortBy/SortDescending nếu có
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                filter.Sort = new List<SortItemDto> 
                { 
                    new SortItemDto { SortKey = filter.SortBy, SortMode = filter.SortDescending == false ? 1 : -1 } 
                };
            }
            else
            {
                filter.Sort = new List<SortItemDto> 
                { 
                    new SortItemDto { SortKey = "CreateDate", SortMode = -1 } 
                };
            }
        }

        var result = await _customerService.SearchAsync(filter);
        return Ok(result);
    }


    /// <summary>
    /// Lấy thông tin khách hàng theo ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(string id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound(new { message = "Customer not found" });
        return Ok(customer);
    }


    [OpenApiOperation("create", "")]
    [HttpPost]

    public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerDto customerDto)

    {
        try
        {
            var created = await _customerService.CreateAsync(customerDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, CustomerDto customerDto)
    {
        try
        {
            await _customerService.UpdateAsync(id, customerDto);
            return NoContent();
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (System.Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa khách hàng (Yêu cầu quyền Admin hoặc Manager)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await _customerService.DeleteAsync(id);
        return NoContent();
    }
}

