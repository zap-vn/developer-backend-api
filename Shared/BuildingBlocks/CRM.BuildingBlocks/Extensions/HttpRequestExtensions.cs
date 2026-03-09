using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Reads the request body stream and deserializes it to the specified type.
        /// Useful for manual list filtering as per latest project standards.
        /// </summary>
        public static async Task<T> GetRawBodyAsync<T>(this HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
            }

            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
                return System.Activator.CreateInstance<T>();

            return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null // Support PascalCase
            }) ?? System.Activator.CreateInstance<T>();
        }
    }
}
