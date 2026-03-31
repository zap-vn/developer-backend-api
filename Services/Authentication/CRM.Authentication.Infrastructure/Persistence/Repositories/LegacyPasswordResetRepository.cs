using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class LegacyPasswordResetRepository : IPasswordResetRepository
    {
        private readonly PostgresDbContext _context;

        public LegacyPasswordResetRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(PasswordResetRequest request)
        {
            await _context.PasswordResetRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetRequest?> GetByResetTokenAsync(string token)
        {
            return await _context.PasswordResetRequests
                .Where(x => x.ResetToken == token)
                .FirstOrDefaultAsync();
        }

        public async Task<PasswordResetRequest?> GetByConfirmTokenAsync(string token)
        {
            return await _context.PasswordResetRequests
                .Where(x => x.ConfirmToken == token)
                .FirstOrDefaultAsync();
        }

        public async Task<PasswordResetRequest?> GetLatestByIdentifierAsync(string identifier)
        {
            return await _context.PasswordResetRequests
                .Where(x => (x.Email == identifier || x.Phone == identifier) && !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PasswordResetRequest request)
        {
            _context.PasswordResetRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetRecentRequestCountAsync(string identifier, DateTime since)
        {
            return await _context.PasswordResetRequests
                .CountAsync(x => (x.Email == identifier || x.Phone == identifier) && x.CreatedAt >= since);
        }
    }
}
