using LegacyDB.Driver;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class LegacySystemConfigRepository : ISystemConfigRepository
    {
        private readonly LegacyDbContext _context;

        public LegacySystemConfigRepository(LegacyDbContext context)
        {
            _context = context;
        }

        public async Task<SystemConfig?> GetByKeyAsync(string key)
        {
            return await _context.SystemConfigs
                .Find(x => x.Key == key)
                .FirstOrDefaultAsync();
        }

        public async Task UpsertAsync(SystemConfig config)
        {
            var options = new FindOneAndReplaceOptions<SystemConfig> { IsUpsert = true };
            await _context.SystemConfigs.FindOneAndReplaceAsync<SystemConfig>(
                x => x.Key == config.Key, 
                config, 
                options);
        }
    }
}
