using ZAP.HR.Domain.Entities;

namespace ZAP.HR.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id);
        Task<Employee?> GetByCodeAsync(string code);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Guid id);
        
        // i18n support
        Task<EmployeeTranslation?> GetTranslationAsync(Guid employeeId, string languageCode);
        Task UpsertTranslationAsync(EmployeeTranslation translation);
    }
}
