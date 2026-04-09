using CRM.Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Product.Domain.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetPagedAsync(LocationListFilter filter);
        Task<int> GetTotalCountAsync(LocationListFilter filter);
        Task<Location?> GetByIdAsync(Guid id);
        Task CreateAsync(Location location);
        Task CreateStoreAsync(Store store);
        Task UpdateAsync(Location location);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<GeoProvince>> GetProvincesAsync(int localeId);
    }
}
