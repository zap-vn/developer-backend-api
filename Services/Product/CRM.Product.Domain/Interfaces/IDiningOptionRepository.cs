using CRM.Product.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Product.Domain.Interfaces
{
    public interface IDiningOptionRepository
    {
        Task<IEnumerable<DiningOption>> GetAllAsync(int localeId);
        Task<DiningOption?> GetByIdAsync(int id, int? localeId);
        Task CreateAsync(DiningOption entity);
        Task UpdateAsync(DiningOption entity);
    }
}
