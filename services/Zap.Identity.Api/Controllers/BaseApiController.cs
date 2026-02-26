using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Zap.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected string CurrentUserGuid
    {
        get
        {
            // First try standard claims from Authentication middleware
            var claim = User.FindFirst("UserGuid");
            if (claim != null) return claim.Value;

            // Fallback: Google API Gateway moves original token to x-forwarded-authorization
            var forwardedAuth = Request.Headers["x-forwarded-authorization"].ToString();
            if (!string.IsNullOrEmpty(forwardedAuth))
            {
                var token = forwardedAuth.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);
                    var userGuid = jwt.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value;
                    if (!string.IsNullOrEmpty(userGuid)) return userGuid;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> ERROR: Failed to parse x-forwarded-authorization token: {ex.Message}");
                }
            }

            Console.WriteLine("--> ERROR: Missing UserGuid claim.");
            Console.WriteLine("--> Available Claims:");
            foreach (var c in User.Claims)
            {
                Console.WriteLine($"   - {c.Type}: {c.Value}");
            }
            throw new UnauthorizedAccessException("Missing UserGuid claim.");
        }
    }

    protected string CurrentLanguage
    {
        get
        {
            var lang = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrEmpty(lang)) return "vi";
            
            lang = lang.ToLower();
            
            // Xử lý các chuỗi đặc biệt (tiếng Việt hoặc tên đầy đủ)
            if (lang.Contains("tiếng hàn") || lang.Contains("korean") || lang.StartsWith("ko")) return "ko";
            if (lang.Contains("tiếng anh") || lang.Contains("english") || lang.StartsWith("en")) return "en";
            if (lang.Contains("tiếng nhật") || lang.Contains("japanese") || lang.StartsWith("ja") || lang.StartsWith("jp")) return "ja";
            if (lang.Contains("tiếng trung") || lang.Contains("chinese") || lang.StartsWith("zh")) return "zh";
            if (lang.Contains("tiếng việt") || lang.Contains("vietnamese") || lang.StartsWith("vi")) return "vi";

            var primaryLang = lang.Split(',')[0].Split(';')[0].Trim();
            return primaryLang.Length >= 2 ? primaryLang.Substring(0, 2) : "vi";
        }
    }

    protected bool IsAdmin
    {
        get
        {
            try {
                var role = User.FindFirst("RoleName")?.Value;
                return role != null && (role.Contains("Admin") || role.Contains("Owner"));
            } catch { return false; }
        }
    }
}

