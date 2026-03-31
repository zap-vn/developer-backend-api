using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using LegacyDB.Driver;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class LegacyEmailSettingRepository : IEmailSettingRepository
    {
        private readonly LegacyDbContext _context;

        public LegacyEmailSettingRepository(LegacyDbContext context)
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
