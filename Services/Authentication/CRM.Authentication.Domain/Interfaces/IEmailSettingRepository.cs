using CRM.Authentication.Domain.Entities;
using System.Threading.Tasks;

namespace CRM.Authentication.Domain.Interfaces
{
    public interface IEmailSettingRepository
    {
        Task<EmailSetting?> GetByCustomerGuidAsync(string customerGuid);
    }
}
