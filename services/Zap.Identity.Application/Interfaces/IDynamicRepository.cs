using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Application.Interfaces;

public interface IDynamicRepository
{
    Task<IEnumerable<BsonDocument>> GetAllAsync(string collectionName, string userGuid, List<FilterItemDto>? filters = null, int limit = 100, int skip = 0, string? sortBy = null, bool sortDescending = false, string? language = "vi");
    Task<BsonDocument?> GetByIdAsync(string collectionName, string id, string userGuid, string? language = "vi");
    Task<BsonDocument> CreateAsync(string collectionName, BsonDocument document, string userGuid);
    Task UpdateAsync(string collectionName, string id, BsonDocument document, string userGuid);
    Task DeleteAsync(string collectionName, string id, string userGuid);
}
