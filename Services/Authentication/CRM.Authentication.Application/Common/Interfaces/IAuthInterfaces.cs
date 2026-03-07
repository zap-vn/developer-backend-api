using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}
