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
        try 
        {
            // 1. Check if MerchantName or Email already exists
            if (!string.IsNullOrEmpty(customer.MerchantName) || !string.IsNullOrEmpty(customer.Email))
            {
                var filterDuplicate = Builders<Customer>.Filter.Or(
                    Builders<Customer>.Filter.Eq(c => c.MerchantName, customer.MerchantName),
                    Builders<Customer>.Filter.Eq(c => c.Email, customer.Email)
                );

                var existing = await _customerCollection.Find(filterDuplicate).FirstOrDefaultAsync();

                if (existing != null)
                {
                    if (existing.MerchantName == customer.MerchantName)
                        throw new InvalidOperationException($"MerchantName '{customer.MerchantName}' already exists.");
                    if (existing.Email == customer.Email)
                        throw new InvalidOperationException($"Email '{customer.Email}' already exists.");
                }
            }

            // 2. Get sequence from ManagementIndex
            var mgmtIndexCol = _customerCollection.Database.GetCollection<BsonDocument>("ManagementIndex");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "Customer_id");
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

            // Set custom ID and Key
            customer.Id = $"Customer/{nextId}";
            customer.Key = nextId;
            customer.CustomerId = nextId; // Syncing CustomerId for consistency
            customer.Revision = Guid.NewGuid().ToString("N").Substring(0, 9); // Mimicking "ad0c70c4E" format

            Console.WriteLine($"--> ATTEMPTING INSERT: _id={customer.Id}, MerchantName={customer.MerchantName}, Email={customer.Email}");
            await _customerCollection.InsertOneAsync(customer);
            Console.WriteLine($"--> INSERT SUCCESSFUL: _id={customer.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> DATABASE ERROR in CreateAsync: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"--> Inner Exception: {ex.InnerException.Message}");
            throw;
        }
    }

    public async Task UpdateAsync(Customer customer)
    {
        // 1. Check if MerchantName or Email is already taken by someone else
        if (!string.IsNullOrEmpty(customer.MerchantName) || !string.IsNullOrEmpty(customer.Email))
        {
            var filterDuplicate = Builders<Customer>.Filter.And(
                Builders<Customer>.Filter.Or(
                    Builders<Customer>.Filter.Eq(c => c.MerchantName, customer.MerchantName),
                    Builders<Customer>.Filter.Eq(c => c.Email, customer.Email)
                ),
                Builders<Customer>.Filter.Ne(c => c.Id, customer.Id) // Exclude current record
            );

            var duplicate = await _customerCollection.Find(filterDuplicate).FirstOrDefaultAsync();
            if (duplicate != null)
            {
                if (duplicate.MerchantName == customer.MerchantName)
                    throw new InvalidOperationException($"MerchantName '{customer.MerchantName}' is already taken by another merchant.");
                if (duplicate.Email == customer.Email)
                    throw new InvalidOperationException($"Email '{customer.Email}' is already taken by another user.");
            }
        }

        var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
        await _customerCollection.ReplaceOneAsync(filter, customer);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
        await _customerCollection.DeleteOneAsync(filter);
    }
}
