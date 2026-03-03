using ZAP.Authentication.Domain.Entities;

namespace ZAP.Authentication.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}
