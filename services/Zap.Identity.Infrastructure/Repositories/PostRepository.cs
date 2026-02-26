using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;
using Zap.Identity.Infrastructure.Persistence;

namespace Zap.Identity.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _postCollection;

    public PostRepository(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
    {
        var databaseName = settings.Value.Databases.TryGetValue("Identity", out var dbName) 
            ? dbName 
            : settings.Value.DatabaseName;
        
        var database = mongoClient.GetDatabase(databaseName);
        _postCollection = database.GetCollection<Post>("Posts");
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _postCollection.Find(p => p.Visible == 1)
            .SortByDescending(p => p.CreateDate)
            .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(string id)
    {
        return await _postCollection.Find(p => p.Id == id && p.Visible == 1).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Post post)
    {
        // Follow the pattern of sequential IDs if needed, but let's use Guid for new collections to avoid complexity
        // unless the user expects sequential IDs. Given the existing code, let's try to be consistent.
        
        var mgmtIndexCol = _postCollection.Database.GetCollection<BsonDocument>("ManagementIndex");
        var filter = Builders<BsonDocument>.Filter.Eq("_id", "Post_id");
        var update = Builders<BsonDocument>.Update
            .Inc("Value", 1)
            .Set("UpdateDate", DateTime.UtcNow.ToString("O"));
        
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var result = await mgmtIndexCol.FindOneAndUpdateAsync(filter, update, options);
        int nextId = result["Value"].ToInt32();

        post.Id = $"Post/{nextId}";
        post.Key = nextId;
        post.Revision = Guid.NewGuid().ToString("N").Substring(0, 9);
        post.Visible = 1;

        await _postCollection.InsertOneAsync(post);
    }

    public async Task UpdateAsync(Post post)
    {
        await _postCollection.ReplaceOneAsync(p => p.Id == post.Id, post);
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Post>.Update.Set(p => p.Visible, 0);
        await _postCollection.UpdateOneAsync(p => p.Id == id, update);
    }
}
