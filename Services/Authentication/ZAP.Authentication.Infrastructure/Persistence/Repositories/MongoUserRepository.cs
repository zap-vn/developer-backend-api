using MongoDB.Driver;
using ZAP.Authentication.Domain.Entities;
using ZAP.Authentication.Domain.Interfaces;

namespace ZAP.Authentication.Infrastructure.Persistence.Repositories
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

        public async Task<User?> GetByUsernameAsync(string username, string merchantCode)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq("Email", username),
                Builders<User>.Filter.Eq("MerchantName", merchantCode)
            );
            
            var user = await _context.Users.Find(filter).FirstOrDefaultAsync();
            
            if (user == null)
            {
                var emailOnlyFilter = Builders<User>.Filter.Eq("Email", username);
                user = await _context.Users.Find(emailOnlyFilter).FirstOrDefaultAsync();
            }

            return user;
        }

        public async Task<bool> ExistsAsync(string email, string username, string merchantName)
        {
            var filters = new System.Collections.Generic.List<FilterDefinition<User>>();
            if (!string.IsNullOrEmpty(email)) filters.Add(Builders<User>.Filter.Eq("Email", email));
            if (!string.IsNullOrEmpty(username)) filters.Add(Builders<User>.Filter.Eq("Username", username));
            if (!string.IsNullOrEmpty(merchantName)) filters.Add(Builders<User>.Filter.Eq("MerchantName", merchantName));

            if (filters.Count == 0) return false;
            
            var orFilter = Builders<User>.Filter.Or(filters);
            return await _context.Users.Find(orFilter).AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.Find(Builders<User>.Filter.Eq("Email", email)).AnyAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.Find(Builders<User>.Filter.Eq("Username", username)).AnyAsync();
        }

        public async Task<bool> MerchantNameExistsAsync(string merchantName)
        {
            return await _context.Users.Find(Builders<User>.Filter.Eq("MerchantName", merchantName)).AnyAsync();
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
    }
}
