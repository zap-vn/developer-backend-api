using MongoDB.Driver;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class MongoOtpRepository : IOtpRepository
    {
        private readonly MongoDbContext _context;

        public MongoOtpRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CustomerOtp otp)
        {
            await _context.CustomerOtps.InsertOneAsync(otp);
        }

        public async Task<CustomerOtp?> GetLatestOtpAsync(string customerId, string purpose)
        {
            return await _context.CustomerOtps
                .Find(o => o.CustomerId == customerId && o.Purpose == purpose)
                .SortByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(CustomerOtp otp)
        {
            await _context.CustomerOtps.ReplaceOneAsync(o => o._id == otp._id, otp);
        }
    }
}
