namespace CRM.Product.Application.Features.Locations.DTOs
{
    public class LocationListRequestDto
    {
        public int page_index { get; set; } = 1;
        public int page_size { get; set; } = 10;
        public string? search { get; set; }
        public LocationFiltersDto? filters { get; set; }
        public LocationSortDto? sort { get; set; }
        public int locale_id { get; set; } = 2; // Default to 2 (VI) or 1 (EN)
    }
    public class LocationFiltersDto
    {
        /// <summary>Trạng thái (status_id): ACTIVE / INACTIVE</summary>
        public int? status_id { get; set; }

        /// <summary>Thành phố / tỉnh (province_id)</summary>
        public int? province_id { get; set; }

        /// <summary>Loại hình vị trí: truyền mảng string, VD: ["1", "2"]</summary>
        public List<string>? location_type_id { get; set; }
    }

    public class LocationSortDto
    {
        /// <summary>Field to sort by: "name" | "node_code" | "status"</summary>
        public string? field { get; set; }

        /// <summary>true = descending (Z-A), false = ascending (A-Z)</summary>
        public bool descending { get; set; } = false;
    }
}
