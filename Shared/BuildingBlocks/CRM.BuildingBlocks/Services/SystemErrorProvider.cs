using Microsoft.Extensions.Caching.Memory;
using CRM.BuildingBlocks.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Services
{
    public class SystemErrorProvider : ISystemErrorProvider
    {
        // MongoDB dependency removed - error messages are handled by ExceptionMiddleware hardcoded fallback
        public Task<SystemError?> GetErrorAsync(string errorCode, string lang)
        {
            return Task.FromResult<SystemError?>(null);
        }

        public Task<int> GetStatusCodeAsync(string errorCode)
        {
            return Task.FromResult(500);
        }
    }
}
