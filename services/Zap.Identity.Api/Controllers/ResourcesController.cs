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
        try 
        {
            Console.WriteLine("--> Fetching setup metadata...");
            var metadata = await _resourceService.GetSetupMetadataAsync();
            Console.WriteLine($"--> Metadata counts: " +
                $"BusinessTypes: {metadata.BusinessTypes?.Count() ?? 0}, " +
                $"Languages: {metadata.Languages?.Count() ?? 0}, " +
                $"TimeZones: {metadata.TimeZones?.Count() ?? 0}, " +
                $"DateFormats: {metadata.DateFormats?.Count() ?? 0}, " +
                $"TimeFormats: {metadata.TimeFormats?.Count() ?? 0}");
            return Ok(metadata);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"--> Error fetching metadata: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
