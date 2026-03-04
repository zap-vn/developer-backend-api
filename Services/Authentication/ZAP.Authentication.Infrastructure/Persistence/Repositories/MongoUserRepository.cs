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
