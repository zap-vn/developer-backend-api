using MongoDB.Driver;
using ZAP.HR.Domain.Entities;
using ZAP.HR.Domain.Interfaces;

namespace ZAP.HR.Infrastructure.Persistence.Repositories
{
    public class MongoEmployeeRepository : IEmployeeRepository
    {
        private readonly MongoDbContext _context;

        public MongoEmployeeRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Employee?> GetByCodeAsync(string code)
        {
            return await _context.Employees.Find(e => e.EmployeeCode == code).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(Employee employee)
        {
            await _context.Employees.InsertOneAsync(employee);
        }

        public async Task UpdateAsync(Employee employee)
        {
            await _context.Employees.ReplaceOneAsync(e => e.Id == employee.Id, employee);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.Employees.DeleteOneAsync(e => e.Id == id);
            await _context.EmployeeTranslations.DeleteManyAsync(t => t.EntityId == id);
        }

        public async Task<EmployeeTranslation?> GetTranslationAsync(Guid employeeId, string languageCode)
        {
            return await _context.EmployeeTranslations.Find(t => t.EntityId == employeeId && t.LanguageCode == languageCode).FirstOrDefaultAsync();
        }

        public async Task UpsertTranslationAsync(EmployeeTranslation translation)
        {
            var filter = Builders<EmployeeTranslation>.Filter.And(
                Builders<EmployeeTranslation>.Filter.Eq(t => t.EntityId, translation.EntityId),
                Builders<EmployeeTranslation>.Filter.Eq(t => t.LanguageCode, translation.LanguageCode)
            );
            await _context.EmployeeTranslations.ReplaceOneAsync(filter, translation, new ReplaceOptions { IsUpsert = true });
        }
    }
}
