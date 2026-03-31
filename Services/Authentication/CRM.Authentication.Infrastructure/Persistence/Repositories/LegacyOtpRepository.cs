using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class LegacyOtpRepository : IOtpRepository
    {
        private readonly PostgresDbContext _context;

        public LegacyOtpRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CustomerOtp otp)
        {
            await _context.CustomerOtps.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpAsync(string customerId, string purpose)
        {
            return await _context.CustomerOtps
                .Where(o => o.CustomerId == customerId && o.Purpose == purpose)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpForPurposesAsync(string customerId, string[] purposes)
        {
            return await _context.CustomerOtps
                .Where(o => o.CustomerId == customerId && purposes.Contains(o.Purpose))
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpByEmailForPurposesAsync(string email, string[] purposes)
        {
            return await _context.CustomerOtps
                .Where(o => (o.Email == email || o.Phone == email) && purposes.Contains(o.Purpose))
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpByPhoneForPurposesAsync(string phone, string[] purposes)
        {
            return await _context.CustomerOtps
                .Where(o => o.Phone == phone && purposes.Contains(o.Purpose))
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(CustomerOtp otp)
        {
            _context.CustomerOtps.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}
