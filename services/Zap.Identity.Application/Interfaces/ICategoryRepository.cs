using System.Collections.Generic;
using System.Threading.Tasks;
using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(string id);
    Task<Category> CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(string id);
}
