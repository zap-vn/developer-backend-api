using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class MongoEmailSettingRepository : IEmailSettingRepository
    {
        private readonly MongoDbContext _context;

        public MongoEmailSettingRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<EmailSetting?> GetByCustomerGuidAsync(string customerGuid)
        {
            return await _context.EmailSettings
                .Find(x => x.CustomerGuid == customerGuid)
                .FirstOrDefaultAsync();
        }
    }
}
