using System.Threading.Tasks;
using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
