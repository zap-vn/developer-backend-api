using Zap.Identity.Domain.Entities;
using Zap.Identity.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zap.Identity.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAndMerchantAsync(string email, string merchantName);
    Task<Customer?> GetByIdAsync(string id);
    Task<IEnumerable<Customer>> GetByIdsAsync(IEnumerable<string> ids);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, List<SortItemDto>? sorts = null);
    Task CreateAsync(Customer customer);

    Task UpdateAsync(Customer customer);
    Task DeleteAsync(string id);
}
