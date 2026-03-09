using MongoDB.Driver;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class MongoPasswordResetRepository : IPasswordResetRepository
    {
        private readonly MongoDbContext _context;

        public MongoPasswordResetRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(PasswordResetRequest request)
        {
            await _context.PasswordResetRequests.InsertOneAsync(request);
        }

        public async Task<PasswordResetRequest?> GetByResetTokenAsync(string token)
        {
            return await _context.PasswordResetRequests
                .Find(x => x.ResetToken == token)
                .FirstOrDefaultAsync();
        }

        public async Task<PasswordResetRequest?> GetByConfirmTokenAsync(string token)
        {
            return await _context.PasswordResetRequests
                .Find(x => x.ConfirmToken == token)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PasswordResetRequest request)
        {
            await _context.PasswordResetRequests.ReplaceOneAsync(x => x.Id == request.Id, request);
        }

        public async Task<int> GetRecentRequestCountAsync(string identifier, DateTime since)
        {
            return (int)await _context.PasswordResetRequests
                .CountDocumentsAsync(x => x.Email == identifier && x.CreatedAt >= since);
        }
    }
}
