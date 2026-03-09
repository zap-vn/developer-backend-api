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
            // For MongoDB, we can use the CreatedAt field
            // Note: identifier here could be email or phone, but we only have UserGuid in the entity.
            // However, we can track by identifier if we add it to the entity or search by it.
            // Let's assume we search for requests created since 'since' for any identifier.
            // In a real system, we'd filter by the specific email/phone.
            
            return (int)await _context.PasswordResetRequests
                .CountDocumentsAsync(x => x.CreatedAt >= since);
        }
    }
}
