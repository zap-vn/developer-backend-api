using System;
using System.Collections.Generic;

namespace CRM.BuildingBlocks.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserGuid { get; }
        string? UserName { get; }
        string LanguageCode { get; }
        IEnumerable<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
