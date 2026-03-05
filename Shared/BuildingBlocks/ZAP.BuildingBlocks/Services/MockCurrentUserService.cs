using System;
using System.Collections.Generic;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.BuildingBlocks.Services
{
    public class MockCurrentUserService : ICurrentUserService
    {
        public string? UserId => Guid.Empty.ToString();
        public string? UserGuid => "Customer/1";
        public string? UserName => "system_admin";
        public string LanguageCode => "vi-VN";
        public IEnumerable<string> Roles => new[] { "Admin" };
        public bool IsAuthenticated => true;
    }
}
