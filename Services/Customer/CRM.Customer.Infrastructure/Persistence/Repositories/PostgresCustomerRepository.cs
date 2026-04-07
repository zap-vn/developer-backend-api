using Microsoft.EntityFrameworkCore;
using CRM.Customer.Domain.Entities;
using CRM.Customer.Domain.Interfaces;
using CRM.Customer.Infrastructure.Persistence;
using CRM.BuildingBlocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Customer.Infrastructure.Persistence.Repositories
{
    public class PostgresCustomerRepository : ICustomerRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresCustomerRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CustomerEntity>> GetPagedAsync(int pageIndex, int pageSize, Guid? tenantId = null, string? search = null)
        {
            var query = _context.Customers.AsQueryable();

            if (tenantId.HasValue)
            {
                query = query.Where(c => c.tenant_id == tenantId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.full_name!.Contains(search) || c.email!.Contains(search) || c.phone_number!.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CustomerEntity>(items, totalCount, pageIndex, pageSize);
        }

        public async Task<CustomerEntity?> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            return await _context.Customers.FirstOrDefaultAsync(c => c.id == guid);
        }

        public async Task<CustomerEntity> CreateAsync(CustomerEntity entity)
        {
            await _context.Customers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(CustomerEntity entity)
        {
            _context.Customers.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return false;
            var customer = await _context.Customers.FindAsync(guid);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
