using CRM.Authentication.Domain.Entities;
using System.Threading.Tasks;

namespace CRM.Authentication.Application.Common.Interfaces
{
    public interface IVietGuyService
    {
        Task SendSmsAsync(string phone, string message, EmailSetting? setting = null);
        Task<string> GetAccessTokenAsync(EmailSetting setting);
        Task RefreshAccessTokenAsync(EmailSetting setting);
    }
}
