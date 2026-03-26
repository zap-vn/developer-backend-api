using System.Threading.Tasks;
using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Domain.Interfaces
{
    public interface ISystemConfigRepository
    {
        Task<SystemConfig?> GetByKeyAsync(string key);
        Task UpsertAsync(SystemConfig config);
    }
}
