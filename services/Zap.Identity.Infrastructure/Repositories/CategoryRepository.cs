using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;
using Zap.Identity.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace Zap.Identity.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categoryCollection;

    public CategoryRepository(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
    {
        var databaseName = settings.Value.Databases.TryGetValue("Identity", out var dbName) 
            ? dbName 
            : settings.Value.DatabaseName;
        
        var database = mongoClient.GetDatabase(databaseName);
        _categoryCollection = database.GetCollection<Category>("Category");
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categoryCollection.Find(_ => true)
            .SortBy(c => c.OrderNo)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(string id)
    {
        return await _categoryCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        await _categoryCollection.InsertOneAsync(category);
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        await _categoryCollection.ReplaceOneAsync(c => c.Id == category.Id, category);
    }

    public async Task DeleteAsync(string id)
    {
        await _categoryCollection.DeleteOneAsync(c => c.Id == id);
    }
}
