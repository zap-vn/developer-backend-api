using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;
using Zap.Identity.Infrastructure.Persistence;

namespace Zap.Identity.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IMongoCollection<Customer> _customerCollection;

    public CustomerRepository(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _customerCollection = database.GetCollection<Customer>("Customer");
    }

    public async Task<Customer?> GetByEmailAndMerchantAsync(string email, string merchantName)
    {
        var filter = Builders<Customer>.Filter.And(
            Builders<Customer>.Filter.Eq(c => c.Email, email),
            Builders<Customer>.Filter.Eq(c => c.MerchantName, merchantName),
            Builders<Customer>.Filter.Eq(c => c.Visible, 1)
        );

        return await _customerCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.CustomerId, customerId);
        return await _customerCollection.Find(filter).FirstOrDefaultAsync();
    }
}
