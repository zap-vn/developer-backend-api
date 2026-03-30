using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class PostgresUserRepository : IUserRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresUserRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            return await _context.Users.FirstOrDefaultAsync(u => u.id == guid);
        }

        public async Task<User?> GetByEmailAndMerchantAsync(string email, string merchantCode)
        {
            // Assuming for now that 'email' or 'phone' can be used as identifier
            // And since merchant relation isn't clear in PG yet, we fallback to email/username
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.Username == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.Username == email);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            // The PG schema screenshot didn't show a phone column.
            // But if it exists in another table or schema, we might need a join.
            // For now, returning null to avoid crash if it's missing.
            return null;
        }

        public async Task<bool> ExistsAsync(string email, string merchantName)
        {
            return await _context.Users.AnyAsync(u => u.Email == email || u.Username == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            // Phone column not found in identity.user screenshot
            return false;
        }

        public async Task<bool> MerchantNameExistsAsync(string merchantName)
        {
            return false;
        }

        public async Task<bool> MerchantUrlExistsAsync(string merchantUrl)
        {
            return false;
        }

        public async Task<long> GetNextSequenceAsync(string sequenceName)
        {
            // Fallback for long-based keys if still used somewhere
            return 0;
        }

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return;
            var user = await _context.Users.FindAsync(guid);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
