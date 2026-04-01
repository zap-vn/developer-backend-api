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
            await _context.Otps.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpAsync(string customerId, string purpose)
        {
            return await _context.Otps
                .Where(o => o.customer_id == customerId && o.purpose == purpose)
                .OrderByDescending(o => o.created_at)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpForPurposesAsync(string customerId, string[] purposes)
        {
            return await _context.Otps
                .Where(o => o.customer_id == customerId && purposes.Contains(o.purpose))
                .OrderByDescending(o => o.created_at)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpByEmailForPurposesAsync(string email, string[] purposes)
        {
            return await _context.Otps
                .Where(o => (o.email == email || o.phone == email) && purposes.Contains(o.purpose))
                .OrderByDescending(o => o.created_at)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerOtp?> GetLatestOtpByPhoneForPurposesAsync(string phone, string[] purposes)
        {
            return await _context.Otps
                .Where(o => o.phone == phone && purposes.Contains(o.purpose))
                .OrderByDescending(o => o.created_at)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(CustomerOtp otp)
        {
            _context.Otps.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}
