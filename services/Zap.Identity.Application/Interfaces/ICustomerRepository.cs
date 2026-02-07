using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAndMerchantAsync(string email, string merchantName);
    Task<Customer?> GetByIdAsync(int customerId);
}
