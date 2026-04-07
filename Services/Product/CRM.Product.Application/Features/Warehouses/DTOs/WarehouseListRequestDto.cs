namespace CRM.Product.Application.Features.Warehouses.DTOs
{
    public class WarehouseListRequestDto
    {
        public int page_index { get; set; } = 1;
        public int page_size { get; set; } = 10;
        public WarehouseSearchDto? search { get; set; }
        public WarehouseFiltersDto? filters { get; set; }
        public WarehouseSortDto? sort { get; set; }
    }

    public class WarehouseSearchDto
    {
        /// <summary>Tên vị trí (name)</summary>
        public string? name { get; set; }

        /// <summary>Mã vị trí: node_code hoặc id</summary>
        public string? location_id { get; set; }

        /// <summary>Địa chỉ (address_line_1)</summary>
        public string? address { get; set; }
    }

    public class WarehouseFiltersDto
    {
        /// <summary>Trạng thái (status_id): ACTIVE / INACTIVE</summary>
        public int? status_id { get; set; }

        /// <summary>Thành phố / tỉnh (province_id)</summary>
        public int? province_id { get; set; }

        /// <summary>Loại hình vị trí (location_type_id / tier_level)</summary>
        public int? location_type_id { get; set; }
    }

    public class WarehouseSortDto
    {
        /// <summary>Field to sort by: "name" | "node_code" | "status"</summary>
        public string? field { get; set; }

        /// <summary>true = descending (Z-A), false = ascending (A-Z)</summary>
        public bool descending { get; set; } = false;
    }
}
