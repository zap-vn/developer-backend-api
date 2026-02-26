using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;
using Zap.Identity.Infrastructure.Persistence;

namespace Zap.Identity.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IMongoCollection<Comment> _commentCollection;

    public CommentRepository(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
    {
        var databaseName = settings.Value.Databases.TryGetValue("Identity", out var dbName) 
            ? dbName 
            : settings.Value.DatabaseName;
        
        var database = mongoClient.GetDatabase(databaseName);
        _commentCollection = database.GetCollection<Comment>("Comments");
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(string postId)
    {
        return await _commentCollection.Find(c => c.PostId == postId && c.Visible == 1)
            .SortBy(c => c.CreateDate)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(string id)
    {
        return await _commentCollection.Find(c => c.Id == id && c.Visible == 1).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Comment comment)
    {
        var mgmtIndexCol = _commentCollection.Database.GetCollection<BsonDocument>("ManagementIndex");
        var filter = Builders<BsonDocument>.Filter.Eq("_id", "Comment_id");
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

        comment.Id = $"Comment/{nextId}";
        comment.Key = nextId;
        comment.Revision = Guid.NewGuid().ToString("N").Substring(0, 9);
        comment.Visible = 1;

        await _commentCollection.InsertOneAsync(comment);
    }

    public async Task UpdateAsync(Comment comment)
    {
        await _commentCollection.ReplaceOneAsync(c => c.Id == comment.Id, comment);
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Comment>.Update.Set(c => c.Visible, 0);
        await _commentCollection.UpdateOneAsync(c => c.Id == id, update);
    }
}
