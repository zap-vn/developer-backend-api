using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class LegacySystemConfigRepository : ISystemConfigRepository
    {
        private readonly PostgresDbContext _context;

        public LegacySystemConfigRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<SystemConfig?> GetByKeyAsync(string key)
        {
            return await _context.SystemConfigs
                .Where(x => x.key == key)
                .FirstOrDefaultAsync();
        }

        public async Task UpsertAsync(SystemConfig config)
        {
            var existing = await _context.SystemConfigs
                .Where(x => x.key == config.key)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await _context.SystemConfigs.AddAsync(config);
            }
            else
            {
                existing.value = config.value;
                _context.SystemConfigs.Update(existing);
            }

            await _context.SaveChangesAsync();
        }
    }
}
