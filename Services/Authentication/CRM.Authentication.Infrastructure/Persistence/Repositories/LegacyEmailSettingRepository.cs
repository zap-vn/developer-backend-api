using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class LegacyEmailSettingRepository : IEmailSettingRepository
    {
        private readonly PostgresDbContext _context;

        public LegacyEmailSettingRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<EmailSetting?> GetByCustomerGuidAsync(string customerGuid)
        {
            return await _context.EmailSettings
                .Where(x => x.customer_guid == customerGuid)
                .FirstOrDefaultAsync();
        }
    }
}
