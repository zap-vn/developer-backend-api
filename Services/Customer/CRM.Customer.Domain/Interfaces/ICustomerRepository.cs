using CRM.Customer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Customer.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CRM.BuildingBlocks.Models.PagedResult<CustomerEntity>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            int? statusId = null,
            Guid? tierId = null,
            decimal? minTotalSpent = null,
            decimal? maxTotalSpent = null,
            decimal? minPoints = null,
            decimal? maxPoints = null,
            string sortField = "full_name",
            bool sortDescending = false);
        Task<CustomerEntity?> GetByIdAsync(string id);
        Task<CustomerEntity> CreateAsync(CustomerEntity entity);
        Task<bool> UpdateAsync(CustomerEntity entity);
        Task<bool> DeleteAsync(string id);
    }
}
