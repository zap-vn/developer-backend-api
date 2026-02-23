using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;

namespace Zap.Identity.Api.Controllers;

public class ResourcesController : BaseApiController
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpPost("setup-metadata")]
    public async Task<ActionResult<IEnumerable<MapResourceDto>>> GetResourceMaps([FromBody] System.Text.Json.JsonElement requestBody)
    {
        try
        {
            var ids = new List<string>();

            // Case 1: Root is a list [ { "_id": ... }, { "_id": ... } ]
            if (requestBody.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var item in requestBody.EnumerateArray())
                {
                    if (item.TryGetProperty("_id", out var idProp)) ids.Add(idProp.GetString() ?? "");
                    else if (item.TryGetProperty("id", out idProp)) ids.Add(idProp.GetString() ?? "");
                }
            }
            // Case 2: Root is an object { "Data": [ ... ] } or { "data": [ ... ] }
            else if (requestBody.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                System.Text.Json.JsonElement dataArray;
                if (requestBody.TryGetProperty("Data", out dataArray) || requestBody.TryGetProperty("data", out dataArray))
                {
                    if (dataArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var item in dataArray.EnumerateArray())
                        {
                            if (item.TryGetProperty("_id", out var idProp)) ids.Add(idProp.GetString() ?? "");
                            else if (item.TryGetProperty("id", out idProp)) ids.Add(idProp.GetString() ?? "");
                        }
                    }
                }
            }

            if (ids.Count == 0)
            {
                return BadRequest("No IDs found in request body. Supported formats: { 'Data': [ { '_id': '...' } ] } or [ { '_id': '...' } ]");
            }

            Console.WriteLine($"--> Fetching resource maps for {ids.Count} IDs for User: {CurrentUserGuid}...");
            var result = await _resourceService.GetResourcesByMapIdsAsync(ids, CurrentUserGuid, CurrentLanguage);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"--> Error processing resource request: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
