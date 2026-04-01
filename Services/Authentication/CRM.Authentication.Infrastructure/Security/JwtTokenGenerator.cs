using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Infrastructure.Security
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly CRM.Authentication.Domain.Interfaces.ISystemConfigRepository _configRepository;

        public JwtTokenGenerator(IConfiguration configuration, CRM.Authentication.Domain.Interfaces.ISystemConfigRepository configRepository)
        {
            _configuration = configuration;
            _configRepository = configRepository;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var keyStr = _configuration["Jwt:Secret"] ?? "a_very_secret_default_key_at_least_32_chars_long";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("UserGuid", user.id.ToString()),
                new Claim("EmployeeGuid", user.id.ToString()),
                new Claim("RoleName", "MerchantAdmin"),
                new Claim("RolePermission_id", "657ab15d54f17333f3d89c65"), 
                new Claim("Language", "vi"),
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.email),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("fullname", user.full_name)
            };

            claims.Add(new Claim(ClaimTypes.Role, "MerchantAdmin"));

            var expiryConfig = await _configRepository.GetByKeyAsync("token_expiry_minutes_crm");
            double expiryMinutes = 120;
            if (expiryConfig != null && double.TryParse(expiryConfig.value, out var parsed)) expiryMinutes = parsed;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
