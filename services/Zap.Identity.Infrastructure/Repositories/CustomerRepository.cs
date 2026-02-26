using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Application.DTOs;
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
        var filter = Builders<Customer>.Filter.And(
            Builders<Customer>.Filter.Eq(c => c.Id, id),
            Builders<Customer>.Filter.Eq(c => c.Visible, 1)
        );
        return await _customerCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Customer>> GetByIdsAsync(IEnumerable<string> ids)
    {
        var filter = Builders<Customer>.Filter.And(
            Builders<Customer>.Filter.In(c => c.Id, ids),
            Builders<Customer>.Filter.Eq(c => c.Visible, 1)
        );
        return await _customerCollection.Find(filter).ToListAsync();
    }


    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _customerCollection.Find(c => c.Visible == 1).ToListAsync();
    }

    public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, List<SortItemDto>? sorts = null)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Visible, 1);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchRegex = new BsonRegularExpression(search, "i");
            var searchFilter = Builders<Customer>.Filter.Or(
                Builders<Customer>.Filter.Regex(c => c.MerchantName, searchRegex),
                Builders<Customer>.Filter.Regex(c => c.Email, searchRegex),
                Builders<Customer>.Filter.Regex(c => c.FirstName, searchRegex),
                Builders<Customer>.Filter.Regex(c => c.LastName, searchRegex),
                Builders<Customer>.Filter.Regex(c => c.BusinessName, searchRegex),
                Builders<Customer>.Filter.Regex(c => c.Phone, searchRegex)
            );
            filter = Builders<Customer>.Filter.And(filter, searchFilter);
        }

        var totalCount = (int)await _customerCollection.CountDocumentsAsync(filter);
        
        var findOptions = _customerCollection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize);

        if (sorts != null && sorts.Any())
        {
            SortDefinition<Customer>? sortDef = null;
            foreach (var s in sorts)
            {
                if (string.IsNullOrEmpty(s.SortKey)) continue;

                var fieldSort = (s.SortMode == -1) // -1 for Descending
                    ? Builders<Customer>.Sort.Descending(s.SortKey) 
                    : Builders<Customer>.Sort.Ascending(s.SortKey);
                
                sortDef = (sortDef == null) ? fieldSort : Builders<Customer>.Sort.Combine(sortDef, fieldSort);
            }
            
            if (sortDef != null)
            {
                findOptions = findOptions.Sort(sortDef);
            }
            else
            {
                findOptions = findOptions.Sort(Builders<Customer>.Sort.Descending(c => c.CreateDate));
            }
        }
        else
        {
            // Default sort
            findOptions = findOptions.Sort(Builders<Customer>.Sort.Descending(c => c.CreateDate).Ascending(c => c.Key));
        }

        var items = await findOptions.ToListAsync();

        return (items, totalCount);
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
        var update = Builders<Customer>.Update.Set(c => c.Visible, 0);
        await _customerCollection.UpdateOneAsync(filter, update);
    }

}
