using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

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
            await _context.PasswordResets.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetRequest?> GetByResetTokenAsync(string token)
        {
            return await _context.PasswordResets
                .Where(x => x.token == token)
                .FirstOrDefaultAsync();
        }

        public async Task<PasswordResetRequest?> GetByConfirmTokenAsync(string token)
        {
            return await _context.PasswordResets
                .Where(x => x.confirm_token == token)
                .FirstOrDefaultAsync();
        }

        public async Task<PasswordResetRequest?> GetLatestByIdentifierAsync(string identifier)
        {
            return await _context.PasswordResets
                .Where(x => (x.email == identifier || x.phone == identifier) && !x.is_used)
                .OrderByDescending(x => x.created_at)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PasswordResetRequest request)
        {
            _context.PasswordResets.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetRecentRequestCountAsync(string identifier, DateTime since)
        {
            return await _context.PasswordResets
                .CountAsync(x => (x.email == identifier || x.phone == identifier) && x.created_at >= since);
        }
    }
}

