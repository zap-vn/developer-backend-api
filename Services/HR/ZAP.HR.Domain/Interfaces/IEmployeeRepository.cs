using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.HR.Domain.Entities;

namespace ZAP.HR.Domain.Interfaces
{
    public interface IEmployeeRepository : IMongoRepository<Employee>
    {
        Task<Employee?> GetByCodeAsync(string code);
        
        // i18n support
        Task<EmployeeTranslation?> GetTranslationAsync(Guid employeeId, string languageCode);
        Task UpsertTranslationAsync(EmployeeTranslation translation);
    }
}
