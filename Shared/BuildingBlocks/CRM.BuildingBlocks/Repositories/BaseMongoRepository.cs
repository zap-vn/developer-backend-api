using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Models;

namespace CRM.BuildingBlocks.Repositories
{
    public abstract class BaseMongoRepository<T> : IMongoRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly ICurrentUserService _currentUserService;

        protected BaseMongoRepository(IMongoDatabase database, string collectionName, ICurrentUserService currentUserService)
        {
            _collection = database.GetCollection<T>(collectionName);
            _currentUserService = currentUserService;
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await _collection.Find(ApplyTenantFilter(x => x.Id == id)).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(ApplyTenantFilter(_ => true)).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(ApplyTenantFilter(predicate)).ToListAsync();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            entity.UserGuid = _currentUserService.UserGuid; // Assign UserGuid (MerchantId) when creating
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            var result = await _collection.ReplaceOneAsync(ApplyTenantFilter(x => x.Id == entity.Id), entity);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public virtual async Task<PagedResult<T>> GetPagedAsync(int pageIndex = 1, int pageSize = 10, Expression<Func<T, bool>>? filter = null)
        {
            var filterDef = filter != null 
                ? ApplyTenantFilter(filter) 
                : ApplyTenantFilter(_ => true);

            var totalCount = await _collection.CountDocumentsAsync(filterDef);
            var items = await _collection.Find(filterDef)
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResult<T>(items, (int)totalCount, pageIndex, pageSize);
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(ApplyTenantFilter(x => x.Id == id));
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Automatically add filter condition for UserGuid (Merchant)
        /// </summary>
        protected FilterDefinition<T> ApplyTenantFilter(Expression<Func<T, bool>> expression)
        {
            var filterBuilder = Builders<T>.Filter;
            var tenantFilter = filterBuilder.Eq(x => x.UserGuid, _currentUserService.UserGuid);
            
            // Combine current filter with tenant filter
            return filterBuilder.And(filterBuilder.Where(expression), tenantFilter);
        }

        protected FilterDefinition<T> ApplyTenantFilter(FilterDefinition<T> filter)
        {
             var filterBuilder = Builders<T>.Filter;
             var tenantFilter = filterBuilder.Eq(x => x.UserGuid, _currentUserService.UserGuid);
             return filterBuilder.And(filter, tenantFilter);
        }
    }
}
