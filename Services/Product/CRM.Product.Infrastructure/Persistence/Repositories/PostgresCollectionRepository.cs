using Microsoft.EntityFrameworkCore;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresCollectionRepository : ICollectionRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresCollectionRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<Collection?> GetByIdAsync(Guid id)
        {
            return await _context.Collections
                .Include(c => c.items)
                .FirstOrDefaultAsync(c => c.id == id);
        }

        public async Task<(IEnumerable<Collection> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null)
        {
            var query = _context.Collections.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.name.Contains(search) || c.slug.Contains(search));
            }

            int total = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.sort_order)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task CreateAsync(Collection collection)
        {
            await _context.Collections.AddAsync(collection);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Collection collection)
        {
            _context.Collections.Update(collection);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection != null)
            {
                _context.Collections.Remove(collection);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddItemsAsync(Guid collectionId, IEnumerable<Guid> productIds)
        {
            var existing = await _context.CollectionItems
                .Where(ci => ci.collection_id == collectionId)
                .Select(ci => ci.product_id)
                .ToListAsync();

            var newIds = productIds.Except(existing);
            
            foreach (var pid in newIds)
            {
                await _context.CollectionItems.AddAsync(new CollectionItem
                {
                    collection_id = collectionId,
                    product_id = pid
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemsAsync(Guid collectionId, IEnumerable<Guid> productIds)
        {
            var items = await _context.CollectionItems
                .Where(ci => ci.collection_id == collectionId && productIds.Contains(ci.product_id))
                .ToListAsync();

            _context.CollectionItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
