using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.HR.Domain.Entities;

namespace CRM.HR.Domain.Interfaces
{
    public interface IEmployeeRepository : IMongoRepository<Employee>
    {
        Task<Employee?> GetByCodeAsync(string code);
        
        // i18n support
        Task<EmployeeTranslation?> GetTranslationAsync(string employeeId, string languageCode);
        Task UpsertTranslationAsync(EmployeeTranslation translation);
    }
}
