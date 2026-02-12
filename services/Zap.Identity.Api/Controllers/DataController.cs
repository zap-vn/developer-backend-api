using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Api.Controllers;

public class DataController : BaseApiController
{
    private readonly IDynamicRepository _dynamicRepository;

    public DataController(IDynamicRepository dynamicRepository)
    {
        _dynamicRepository = dynamicRepository;
    }

    [HttpGet("{collectionName}")]
    public async Task<IActionResult> List(string collectionName, [FromQuery] int limit = 100, [FromQuery] int skip = 0)
    {
        var docs = await _dynamicRepository.GetAllAsync(collectionName, CurrentUserGuid, null, limit, skip, null, false, CurrentLanguage);
        
        var result = docs.Select(doc => {
            var sortedDoc = new BsonDocument(doc.OrderBy(x => x.Name));
            return BsonTypeMapper.MapToDotNetValue(sortedDoc);
        });

        return Ok(result);
    }

    [HttpPost("find/{collectionName}")]
    public async Task<IActionResult> Find(string collectionName, [FromBody] FilterDto filterDto, [FromQuery] int limit = 100, [FromQuery] int skip = 0, [FromQuery] string? sortBy = null, [FromQuery] bool sortDescending = false)
    {
        var docs = await _dynamicRepository.GetAllAsync(collectionName, CurrentUserGuid, filterDto.Filter, limit, skip, sortBy, sortDescending, CurrentLanguage);
        
        var result = docs.Select(doc => {
            var sortedDoc = new BsonDocument(doc.OrderBy(x => x.Name));
            return BsonTypeMapper.MapToDotNetValue(sortedDoc);
        });

        return Ok(result);
    }

    [HttpGet("{collectionName}/{id}")]
    public async Task<IActionResult> Get(string collectionName, string id)
    {
        try
        {
            Console.WriteLine($"--> Getting {collectionName} with ID: {id}");
            // Use property accessors to trigger logs if missing
            var userGuid = CurrentUserGuid; 
            var lang = CurrentLanguage;
            Console.WriteLine($"--> User: {userGuid}, Lang: {lang}");

            var doc = await _dynamicRepository.GetByIdAsync(collectionName, id, userGuid, lang);
            
            if (doc == null) 
            {
                Console.WriteLine("--> Document not found.");
                return NotFound();
            }

            var sortedDoc = new BsonDocument(doc.OrderBy(x => x.Name));
            return Ok(BsonTypeMapper.MapToDotNetValue(sortedDoc));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error in DataController.Get: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw; // Re-throw to be caught by global handler
        }
    }

    [HttpPost("{collectionName}")]
    public async Task<IActionResult> Create(string collectionName, [FromBody] IDictionary<string, object> data)
    {
        var doc = new BsonDocument();
        foreach (var kvp in data)
        {
            doc.Add(kvp.Key, BsonValue.Create(kvp.Value));
        }

        var created = await _dynamicRepository.CreateAsync(collectionName, doc, CurrentUserGuid);
        return Ok(BsonTypeMapper.MapToDotNetValue(created));
    }

    [HttpPut("{collectionName}/{id}")]
    public async Task<IActionResult> Update(string collectionName, string id, [FromBody] IDictionary<string, object> data)
    {
        var doc = new BsonDocument();
        foreach (var kvp in data)
        {
            if (kvp.Key == "_id") continue;
            doc.Add(kvp.Key, BsonValue.Create(kvp.Value));
        }

        await _dynamicRepository.UpdateAsync(collectionName, id, doc, CurrentUserGuid);
        return NoContent();
    }

    [HttpDelete("{collectionName}/{id}")]
    public async Task<IActionResult> Delete(string collectionName, string id)
    {
        await _dynamicRepository.DeleteAsync(collectionName, id, CurrentUserGuid);
        return NoContent();
    }
}
