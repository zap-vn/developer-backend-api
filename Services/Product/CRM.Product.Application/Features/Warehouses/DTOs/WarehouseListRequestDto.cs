using System.Collections.Generic;

namespace CRM.Product.Application.Features.Warehouses.DTOs
{
    public class WarehouseListRequestDto
    {
        public int page_index { get; set; } = 1;
        public int page_size { get; set; } = 10;
        public string? search { get; set; }
        public WarehouseFilters? filters { get; set; }
    }

    public class WarehouseFilters
    {
        public List<int>? status { get; set; }
        public bool? is_active { get; set; }
    }
}
