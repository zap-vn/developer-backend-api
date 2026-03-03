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
            Console.WriteLine($"[MongoDB Diagnostic] Searching for Username: '{username}', Merchant: '{merchantCode}'");
            
            // Try combined filter
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq("Email", username),
                Builders<User>.Filter.Eq("MerchantName", merchantCode)
            );
            
            var user = await _context.Users.Find(filter).FirstOrDefaultAsync();
            
            if (user == null)
            {
                Console.WriteLine($"[MongoDB Diagnostic] Not found with Email={username} AND MerchantName={merchantCode}. Trying Email only...");
                
                var emailOnlyFilter = Builders<User>.Filter.Eq("Email", username);
                user = await _context.Users.Find(emailOnlyFilter).FirstOrDefaultAsync();
                
                if (user != null)
                {
                    Console.WriteLine($"[MongoDB Diagnostic] FOUND by Email only! User's MerchantName in DB is: '{user.MerchantName}'");
                }
                else
                {
                    Console.WriteLine("[MongoDB Diagnostic] Still not found. Listing first 5 documents in 'Customer' collection for inspection:");
                    try 
                    {
                        var allDocs = await _context.Users.Find(new MongoDB.Bson.BsonDocument()).Limit(5).ToListAsync();
                        foreach(var doc in allDocs)
                        {
                            Console.WriteLine($"[DB Sample] ID: {doc._id}, Email: {doc.Username}, Merchant: {doc.MerchantName}, Visible: {doc.Visible}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DB Error] Failed to scan collection: {ex.Message}");
                    }
                }
            }
            else 
            {
                Console.WriteLine($"[MongoDB Diagnostic] Successfully found user: {user._id}");
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
