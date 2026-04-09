using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.BuildingBlocks.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? UserGuid => _httpContextAccessor.HttpContext?.User?.FindFirstValue("UserGuid");

        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        public string LanguageCode => _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].FirstOrDefault()?.Split(',')[0] ?? "vi-VN";
        public int LocaleId 
        {
            get 
            {
                if (int.TryParse(LanguageCode, out var id)) return id;
                return LanguageCode.StartsWith("vi") ? 2 : 1;
            }
        }
        public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value) ?? Enumerable.Empty<string>();

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
