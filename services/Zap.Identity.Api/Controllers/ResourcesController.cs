using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;

namespace Zap.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpGet("setup-metadata")]
    public async Task<ActionResult<SetupMetadataDto>> GetSetupMetadata()
    {
        var metadata = await _resourceService.GetSetupMetadataAsync();
        return Ok(metadata);
    }
}
