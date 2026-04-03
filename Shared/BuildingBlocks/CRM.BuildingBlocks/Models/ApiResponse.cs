using System.Text.Json.Serialization;

namespace CRM.BuildingBlocks.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("pagination")]
        public PaginationMetadata? Pagination { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        public static ApiResponse<T> SuccessResult(T? data, PaginationMetadata? pagination = null, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Pagination = pagination,
                Message = message,
                Metadata = new Dictionary<string, object> { { "server_time", System.DateTime.UtcNow } }
            };
        }

        public static ApiResponse<T> FailureResult(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Metadata = new Dictionary<string, object> { { "server_time", System.DateTime.UtcNow } }
            };
        }
    }

    public class PaginationMetadata
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        public PaginationMetadata(int page, int pageSize, int totalCount)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
