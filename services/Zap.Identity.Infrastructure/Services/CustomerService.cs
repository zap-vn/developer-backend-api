using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PagedResult<CustomerDto>> SearchAsync(FilterDto filter)
    {
        // Standardize pagination
        int limit = filter.Limit > 0 ? filter.Limit : 20;
        int skip = filter.Skip >= 0 ? filter.Skip : 0;
        
        // Calculate page for the repository (1-indexed)
        int page = (skip / limit) + 1;
        int pageSize = limit;

        // Use global Search first, if missing check filters for legacy support
        string? search = filter.Search;
        if (string.IsNullOrEmpty(search) && filter.Filter != null && filter.Filter.Any())
        {
            var searchFilter = filter.Filter.FirstOrDefault(f => 
                string.IsNullOrEmpty(f.SearchKey) || 
                f.SearchKey.Equals("Search", StringComparison.OrdinalIgnoreCase) ||
                f.SearchKey.Equals("Global", StringComparison.OrdinalIgnoreCase));
            
            search = searchFilter?.Value?.ToString();
        }

        var (items, totalCount) = await _customerRepository.GetPagedAsync(page, pageSize, search, filter.Sort);
        
        return new PagedResult<CustomerDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }




    public async Task<CustomerDto?> GetByIdAsync(string id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<CustomerDto> CreateAsync(CustomerDto customerDto)
    {
        var customer = MapToEntity(customerDto);
        customer.CreateDate = DateTime.UtcNow.ToString("O");
        customer.Visible = 1;
        
        await _customerRepository.CreateAsync(customer);
        return MapToDto(customer);
    }

    public async Task UpdateAsync(string id, CustomerDto customerDto)
    {
        var existing = await _customerRepository.GetByIdAsync(id);
        if (existing == null) throw new Exception("Customer not found");

        // Update fields
        existing.MerchantName = customerDto.MerchantName ?? existing.MerchantName;
        existing.ProjectName = customerDto.ProjectName ?? existing.ProjectName;
        existing.BusinessType = customerDto.BusinessType ?? existing.BusinessType;
        existing.UseAiContentGeneration = customerDto.UseAiContentGeneration;
        existing.Language = customerDto.Language ?? existing.Language;
        existing.DateFormat = customerDto.DateFormat ?? existing.DateFormat;
        existing.TimeFormat = customerDto.TimeFormat ?? existing.TimeFormat;
        existing.TimeZoneId = customerDto.TimeZoneId ?? existing.TimeZoneId;
        existing.TimeZoneDisplayName = customerDto.TimeZoneDisplayName ?? existing.TimeZoneDisplayName;
        existing.Country = customerDto.Country ?? existing.Country;
        existing.ReferenceAssets = customerDto.ReferenceAssets;
        existing.FirstName = customerDto.FirstName ?? existing.FirstName;
        existing.LastName = customerDto.LastName ?? existing.LastName;
        existing.Email = customerDto.Email ?? existing.Email;
        existing.Phone = customerDto.Phone ?? existing.Phone;

        await _customerRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(string id)
    {
        await _customerRepository.DeleteAsync(id);
    }

    private CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            CustomerCode = customer.CustomerCode,
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            BusinessName = customer.BusinessName,
            MerchantName = customer.MerchantName,
            ProjectName = customer.ProjectName,
            BusinessType = customer.BusinessType,
            UseAiContentGeneration = customer.UseAiContentGeneration,
            Language = customer.Language,
            DateFormat = customer.DateFormat,
            TimeFormat = customer.TimeFormat,
            TimeZoneId = customer.TimeZoneId,
            TimeZoneDisplayName = customer.TimeZoneDisplayName,
            Country = customer.Country,
            ReferenceAssets = customer.ReferenceAssets,
            Phone = customer.Phone,
            CustomerStatusId = customer.CustomerStatusId
        };
    }

    private Customer MapToEntity(CustomerDto dto)
    {
        return new Customer
        {
            Id = dto.Id ?? string.Empty,
            CustomerCode = dto.CustomerCode ?? string.Empty,
            Email = dto.Email,
            FirstName = dto.FirstName ?? string.Empty,
            LastName = dto.LastName ?? string.Empty,
            BusinessName = dto.BusinessName ?? string.Empty,
            MerchantName = dto.MerchantName ?? string.Empty,
            ProjectName = dto.ProjectName ?? string.Empty,
            BusinessType = dto.BusinessType ?? string.Empty,
            UseAiContentGeneration = dto.UseAiContentGeneration,
            Language = dto.Language ?? string.Empty,
            DateFormat = dto.DateFormat ?? string.Empty,
            TimeFormat = dto.TimeFormat ?? string.Empty,
            TimeZoneId = dto.TimeZoneId ?? string.Empty,
            TimeZoneDisplayName = dto.TimeZoneDisplayName ?? string.Empty,
            Country = dto.Country ?? 0,
            ReferenceAssets = dto.ReferenceAssets,
            Phone = dto.Phone ?? string.Empty,
            CustomerStatusId = dto.CustomerStatusId
        };
    }
}
