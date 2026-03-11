using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Interfaces
{
    public interface ISystemErrorProvider
    {
        Task<SystemError?> GetErrorAsync(string errorCode, string lang);
        Task<int> GetStatusCodeAsync(string errorCode);
    }
}
