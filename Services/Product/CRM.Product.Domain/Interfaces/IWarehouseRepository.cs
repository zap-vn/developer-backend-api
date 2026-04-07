using CRM.Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Product.Domain.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<Warehouse>> GetPagedAsync(LocationListFilter filter);
        Task<int> GetTotalCountAsync(LocationListFilter filter);
        Task<Warehouse?> GetByIdAsync(Guid id);
        Task CreateAsync(Warehouse warehouse);
        Task CreateStoreAsync(Store store);
        Task UpdateAsync(Warehouse warehouse);
        Task DeleteAsync(Guid id);
    }
}
