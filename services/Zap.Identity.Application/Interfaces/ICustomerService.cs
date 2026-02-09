using System.Collections.Generic;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(string id);
    Task<CustomerDto> CreateAsync(CustomerDto customerDto);
    Task UpdateAsync(string id, CustomerDto customerDto);
    Task DeleteAsync(string id);
}
