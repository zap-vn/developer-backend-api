using MongoDB.Driver;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly MongoDbContext _context;

        public MongoUserRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users.Find(u => u._id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAndMerchantAsync(string email, string merchantCode)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Or(
                    Builders<User>.Filter.Eq("Email", email),
                    Builders<User>.Filter.Eq("Phone", email)
                ),
                Builders<User>.Filter.Eq("MerchantName", merchantCode)
            );
            
            return await _context.Users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _context.Users.Find(u => u.Phone == phone).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsAsync(string email, string merchantName)
        {
            var filters = new System.Collections.Generic.List<FilterDefinition<User>>();
            if (!string.IsNullOrEmpty(email)) filters.Add(Builders<User>.Filter.Eq("Email", email));
            if (!string.IsNullOrEmpty(merchantName)) filters.Add(Builders<User>.Filter.Eq("MerchantName", merchantName));

            if (filters.Count == 0) return false;
            
            var orFilter = Builders<User>.Filter.Or(filters);
            return await _context.Users.Find(orFilter).AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.Find(Builders<User>.Filter.Eq("Email", email)).AnyAsync();
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Users.Find(Builders<User>.Filter.Eq("Phone", phone)).AnyAsync();
        }

        public async Task<bool> MerchantNameExistsAsync(string merchantName)
        {
            return await _context.Users.Find(Builders<User>.Filter.Eq("MerchantName", merchantName)).AnyAsync();
        }

        public async Task<bool> MerchantUrlExistsAsync(string merchantUrl)
        {
            return await _context.Users.Find(Builders<User>.Filter.Eq("MerchantUrl", merchantUrl)).AnyAsync();
        }

        public async Task<long> GetNextSequenceAsync(string sequenceName)
        {
            var filter = Builders<ManagementIndex>.Filter.Eq(x => x._id, sequenceName);
            var update = Builders<ManagementIndex>.Update.Inc(x => x.Value, 1);
            var options = new FindOneAndUpdateOptions<ManagementIndex>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var result = await _context.ManagementIndexes.FindOneAndUpdateAsync(filter, update, options);
            return result.Value;
        }

        public async Task<TenantNode?> GetTenantBySlugAsync(string slug)
        {
            // Note: TenantNode is primarily managed in PostgreSQL for Unified Omni-Tier
            // This is a placeholder for MongoDB if ever needed
            return await Task.FromResult<TenantNode?>(null);
        }

        public async Task CreateTenantNodeAsync(TenantNode node)
        {
            // Note: TenantNode creation is handled in PostgreSQL repository
            await Task.CompletedTask;
        }

        public async Task CreateAsync(User user)
        {
            await _context.Users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _context.Users.ReplaceOneAsync(u => u._id == user._id, user);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.Users.DeleteOneAsync(u => u._id == id);
        }

        public Task BeginTransactionAsync() => Task.CompletedTask;
        public Task CommitTransactionAsync() => Task.CompletedTask;
        public Task RollbackTransactionAsync() => Task.CompletedTask;
    }
}
