using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
