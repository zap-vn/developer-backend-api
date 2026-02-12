using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;

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
            var claim = User.FindFirst("UserGuid");
            if (claim == null) 
            {
                Console.WriteLine("--> ERROR: Missing UserGuid claim.");
                Console.WriteLine("--> Available Claims:");
                foreach (var c in User.Claims)
                {
                    Console.WriteLine($"   - {c.Type}: {c.Value}");
                }
                throw new UnauthorizedAccessException("Missing UserGuid claim.");
            }
            return claim.Value;
        }
    }

    protected string CurrentLanguage
    {
        get
        {
            var lang = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrEmpty(lang)) return "vi";
            
            var primaryLang = lang.Split(',')[0].Split(';')[0].Trim();
            return primaryLang.Length >= 2 ? primaryLang.Substring(0, 2).ToLower() : "vi";
        }
    }
}
