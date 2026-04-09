using CRM.Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Product.Domain.Interfaces
{
    public interface ICollectionRepository
    {
        Task<Collection?> GetByIdAsync(Guid id);
        Task<(IEnumerable<Collection> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null);
        Task CreateAsync(Collection collection);
        Task UpdateAsync(Collection collection);
        Task DeleteAsync(Guid id);
        Task AddItemsAsync(Guid collectionId, IEnumerable<Guid> productIds);
        Task RemoveItemsAsync(Guid collectionId, IEnumerable<Guid> productIds);
    }
}
