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

        public async Task<PagedResult<CustomerEntity>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            int? statusId = null,
            int? tierId = null,
            decimal? minTotalSpent = null,
            decimal? maxTotalSpent = null,
            decimal? minPoints = null,
            decimal? maxPoints = null,
            string sortField = "full_name",
            bool sortDescending = false)
        {
            var query = _context.Customers
                .Include(c => c.loyalty_tier)
                .AsNoTracking()
                .AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(c => c.tenant_id == tenantId.Value);

            if (statusId.HasValue)
                query = query.Where(c => c.status_id == statusId.Value);

            if (tierId.HasValue)
                query = query.Where(c => c.tier_id == tierId.Value);

            if (minTotalSpent.HasValue)
                query = query.Where(c => c.total_spent_amount >= minTotalSpent.Value);
            if (maxTotalSpent.HasValue)
                query = query.Where(c => c.total_spent_amount <= maxTotalSpent.Value);

            if (minPoints.HasValue)
                query = query.Where(c => c.current_points_balance >= minPoints.Value);
            if (maxPoints.HasValue)
                query = query.Where(c => c.current_points_balance <= maxPoints.Value);

            // Search: full_name, phone_number, legacy_id, or exact Guid id
            if (!string.IsNullOrEmpty(search))
            {
                Guid? searchGuid = Guid.TryParse(search, out var sg) ? sg : (Guid?)null;
                query = searchGuid.HasValue
                    ? query.Where(c => c.id == searchGuid)
                    : query.Where(c =>
                        (c.full_name != null && c.full_name.Contains(search)) ||
                        (c.phone_number != null && c.phone_number.Contains(search)) ||
                        (c.legacy_id != null && c.legacy_id.Contains(search)));
            }

            var totalCount = await query.CountAsync();

            IOrderedQueryable<CustomerEntity> ordered = sortField switch
            {
                "total_spent_amount" => sortDescending
                    ? query.OrderByDescending(c => c.total_spent_amount)
                    : query.OrderBy(c => c.total_spent_amount),
                "current_points_balance" => sortDescending
                    ? query.OrderByDescending(c => c.current_points_balance)
                    : query.OrderBy(c => c.current_points_balance),
                "tier_id" => sortDescending
                    ? query.OrderByDescending(c => c.tier_id)
                    : query.OrderBy(c => c.tier_id),
                "status_id" => sortDescending
                    ? query.OrderByDescending(c => c.status_id)
                    : query.OrderBy(c => c.status_id),
                _ => sortDescending
                    ? query.OrderByDescending(c => c.full_name)
                    : query.OrderBy(c => c.full_name),
            };

            var items = await ordered
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
