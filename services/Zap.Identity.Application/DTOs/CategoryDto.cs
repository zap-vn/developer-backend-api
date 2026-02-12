namespace Zap.Identity.Application.DTOs;

public class CategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SubCategory { get; set; }
    public int ParentCategoryId { get; set; }
    public int Level { get; set; }
    public string Color { get; set; } = "#ccc";
    public string Acronymn { get; set; } = string.Empty;
    public int BusinessTypeId { get; set; }
    public string Handle { get; set; } = string.Empty;
    public string Ansi { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int OrderNo { get; set; }
    public int Visible { get; set; }
}

public class CreateCategoryDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ParentCategoryId { get; set; }
    public string Color { get; set; } = "#ccc";
    public int BusinessTypeId { get; set; }
    public int OrderNo { get; set; }
    public int Visible { get; set; } = 1;
}

public class UpdateCategoryDto : CreateCategoryDto
{
    public string Id { get; set; } = string.Empty;
}
