using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.HR.Domain.Entities;
using ZAP.HR.Domain.Interfaces;

namespace ZAP.HR.Infrastructure.Persistence.Repositories
{
    public class MongoEmployeeRepository : BaseMongoRepository<Employee>, IEmployeeRepository
    {
        public MongoEmployeeRepository(MongoDbContext context, ICurrentUserService currentUserService) 
            : base(context.Database, "Employees", currentUserService)
        {
        }

        public async Task<Employee?> GetByCodeAsync(string code)
        {
            return await _collection.Find(ApplyTenantFilter(e => e.EmployeeCode == code)).FirstOrDefaultAsync();
        }

        public async Task<EmployeeTranslation?> GetTranslationAsync(Guid employeeId, string languageCode)
        {
            // Note: In the new pattern, translations are embedded. 
            // This method is for backward compatibility or specific lookups.
            var employee = await GetByIdAsync(employeeId);
            return employee?.Translations?.FirstOrDefault(t => t.LanguageCode == languageCode);
        }

        public async Task UpsertTranslationAsync(EmployeeTranslation translation)
        {
            var filter = ApplyTenantFilter(e => e.Id == translation.EntityId);
            var employee = await _collection.Find(filter).FirstOrDefaultAsync();
            if (employee != null)
            {
                if (employee.Translations == null) employee.Translations = new List<EmployeeTranslation>();
                
                var existing = employee.Translations.FirstOrDefault(t => t.LanguageCode == translation.LanguageCode);
                if (existing != null)
                {
                    employee.Translations.Remove(existing);
                }
                employee.Translations.Add(translation);
                
                await UpdateAsync(employee);
            }
        }

        // Override Create to ensure fallback values are set if needed
        public override async Task<Employee> CreateAsync(Employee entity)
        {
            return await base.CreateAsync(entity);
        }
    }
}
