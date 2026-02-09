using Zap.Identity.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zap.Identity.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAndMerchantAsync(string email, string merchantName);
    Task<Customer?> GetByIdAsync(string id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(string id);
}
