using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.Product.Domain.Entities;
using ZAP.Product.Domain.Interfaces;

namespace ZAP.Product.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MongoDbContext _context;

        public ProductRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<ProductEntity> GetByIdAsync(Guid id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProductEntity>> GetAllAsync()
        {
            return await _context.Products.Find(_ => true).ToListAsync();
        }

        public async Task<ProductEntity> CreateAsync(ProductEntity product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> UpdateAsync(ProductEntity product)
        {
            var result = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category)
        {
            return await _context.Products.Find(p => p.Category == category).ToListAsync();
        }
    }
}
