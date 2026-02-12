using System.Collections.Generic;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(string id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, string userGuid);
    Task UpdateAsync(UpdateCategoryDto dto, string userGuid);
    Task DeleteAsync(string id);
}
