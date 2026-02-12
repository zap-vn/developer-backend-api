using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(string id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, string userGuid)
    {
        var existing = await _categoryRepository.GetAllAsync();
        int maxKey = existing.Any() ? existing.Max(c => c.Key) : 0;
        int nextKey = maxKey + 1;

        var category = new Category
        {
            Id = $"Category/{nextKey}",
            Key = nextKey,
            Title = dto.Title,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            Color = dto.Color,
            BusinessTypeId = dto.BusinessTypeId,
            OrderNo = dto.OrderNo,
            Visible = dto.Visible,
            UserGuid = userGuid,
            CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            AdminInsert = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Handle = dto.Title.ToLower().Replace(" ", "-"),
            Ansi = dto.Title.ToLower().Replace(" ", "-"),
            Version = DateTime.Now.ToString("yyMMddHHmmss"),
            Level = dto.ParentCategoryId == 0 ? 1 : 2
        };

        var created = await _categoryRepository.CreateAsync(category);
        return MapToDto(created);
    }

    public async Task UpdateAsync(UpdateCategoryDto dto, string userGuid)
    {
        var existing = await _categoryRepository.GetByIdAsync(dto.Id);
        if (existing == null) throw new KeyNotFoundException("Category not found");

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.ParentCategoryId = dto.ParentCategoryId;
        existing.Color = dto.Color;
        existing.BusinessTypeId = dto.BusinessTypeId;
        existing.OrderNo = dto.OrderNo;
        existing.Visible = dto.Visible;
        existing.Version = DateTime.Now.ToString("yyMMddHHmmss");
        existing.Handle = dto.Title.ToLower().Replace(" ", "-");
        existing.Ansi = dto.Title.ToLower().Replace(" ", "-");

        await _categoryRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(string id)
    {
        await _categoryRepository.DeleteAsync(id);
    }

    private CategoryDto MapToDto(Category c) => new CategoryDto
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        ReferenceId = c.ReferenceId,
        SubCategory = c.SubCategory,
        ParentCategoryId = c.ParentCategoryId,
        Level = c.Level,
        Color = c.Color,
        Acronymn = c.Acronymn,
        BusinessTypeId = c.BusinessTypeId,
        Handle = c.Handle,
        Ansi = c.Ansi,
        CategoryId = c.CategoryId,
        OrderNo = c.OrderNo,
        Visible = c.Visible
    };
}
