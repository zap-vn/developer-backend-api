using MongoDB.Driver;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;

namespace CRM.BuildingBlocks.Repositories
{
    public class SystemLanguageRepository : BaseMongoRepository<SystemLanguage>
    {
        public SystemLanguageRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "SystemLanguages", currentUserService)
        {
        }
        
        // Overriding tenant filter because SystemLanguages are global
        public override async Task<IEnumerable<SystemLanguage>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public override async Task<IEnumerable<SystemLanguage>> FindAsync(System.Linq.Expressions.Expression<System.Func<SystemLanguage, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }
    }
}
