using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
        email = (email ?? "").Trim();
        merchantName = (merchantName ?? "").Trim();

        // Create a case-insensitive regex for email
        var emailRegex = new BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(email)}$", "i");
        
        FilterDefinition<Customer> merchantFilter;
        if (string.IsNullOrEmpty(merchantName))
        {
            merchantFilter = Builders<Customer>.Filter.Or(
                Builders<Customer>.Filter.Eq(c => c.MerchantName, ""),
                Builders<Customer>.Filter.Eq(c => c.MerchantName, "\"\""),
                Builders<Customer>.Filter.Eq(c => c.MerchantName, null)
            );
        }
        else
        {
            // Case-insensitive merchant name matching
            merchantFilter = Builders<Customer>.Filter.Regex(c => c.MerchantName, 
                new BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(merchantName)}$", "i"));
        }

        var filter = Builders<Customer>.Filter.And(
            Builders<Customer>.Filter.Regex(c => c.Email, emailRegex),
            merchantFilter
        );

        Console.WriteLine($"--> Searching for: Email='{email}', Merchant='{merchantName}'");
        var customer = await _customerCollection.Find(filter).FirstOrDefaultAsync();
        
        if (customer == null)
        {
            Console.WriteLine("--> DEBUG: NO CUSTOMER FOUND with this email/merchant combination.");
            // Extra debug: list all customers with this email regardless of merchant
            var anyWithEmail = await _customerCollection.Find(Builders<Customer>.Filter.Regex(c => c.Email, emailRegex)).ToListAsync();
            Console.WriteLine($"--> DEBUG: Found {anyWithEmail.Count} customers with this email total.");
            foreach(var c in anyWithEmail) {
                Console.WriteLine($"    - DB Match: Email='{c.Email}', Merchant='{c.MerchantName}', Visible={c.Visible}");
            }
        }
        else
        {
            Console.WriteLine($"--> DEBUG: Customer found! Email={customer.Email}, Merchant={customer.MerchantName}");
        }

        return customer;
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
        return await _customerCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _customerCollection.Find(_ => true).ToListAsync();
    }

    public async Task CreateAsync(Customer customer)
    {
        if (string.IsNullOrEmpty(customer.Id))
        {
            customer.Id = Guid.NewGuid().ToString();
        }
        await _customerCollection.InsertOneAsync(customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
        await _customerCollection.ReplaceOneAsync(filter, customer);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
        await _customerCollection.DeleteOneAsync(filter);
    }
}
