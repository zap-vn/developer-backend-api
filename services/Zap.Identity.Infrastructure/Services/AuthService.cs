using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;
using Zap.Identity.Infrastructure.Settings;

namespace Zap.Identity.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ICustomerRepository customerRepository, IOptions<JwtSettings> jwtSettings)
    {
        _customerRepository = customerRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Find customer by email and merchant name
        var customer = await _customerRepository.GetByEmailAndMerchantAsync(
            request.UserName, 
            request.MerchantName);

        if (customer == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid credentials. Customer not found."
            };
        }

        // Verify password
        var hashedPassword = HashPassword(request.Password);
        if (customer.Password != hashedPassword)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid credentials. Incorrect password."
            };
        }

        // Check if customer is active
        if (customer.CustomerStatusId != 1 || customer.Visible != 1)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Account is not active."
            };
        }

        // Generate JWT token
        var token = GenerateJwtToken(customer, request.IsRemember);

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            MerchantName = customer.MerchantName,
            AccessToken = token,
            Avatar = customer.Url,
            ExpiresIn = (request.IsRemember ? _jwtSettings.ExpirationInMinutes * 24 : _jwtSettings.ExpirationInMinutes) * 60,
            FullName = $"{customer.FirstName} {customer.LastName}".Trim(),
            RefreshToken = Guid.NewGuid().ToString(),
            Role = "Owner (Super Admin)",
            UserGuid = $"Customer/{customer.CustomerId}"
        };
    }

    private string HashPassword(string password)
    {
        // 1. Get MD5 Hash (lowercase hex)
        string md5Hash = GetMd5Hash(password);
        
        // 2. Generate Salted SHA256 Hash
        return GenerateSaltedHash(md5Hash);
    }

    private string GetMd5Hash(string input)
    {
        using var md5 = MD5.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = md5.ComputeHash(bytes);

        var sb = new StringBuilder();
        foreach (byte b in hash)
        {
            sb.Append(b.ToString("x2").ToLower());
        }

        return sb.ToString();
    }

    private string GenerateSaltedHash(string input)
    {
        string salt = "admin@backend.api.vn";
        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private string GenerateJwtToken(Customer customer, bool isRemember)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Map claims to match the requested legacy/required structure
        var claims = new[]
        {
            new Claim("UserGuid", $"Customer/{customer.CustomerId}"),
            new Claim("EmployeeGuid", $"Customer/{customer.CustomerId}"),
            new Claim("RoleName", "Owner (Super Admin)"),
            new Claim("RolePermission_id", "657ab15d54f17333f3d89c65"), // From user example
            new Claim("Language", string.IsNullOrEmpty(customer.Language) ? "vi" : customer.Language),
            new Claim(JwtRegisteredClaimNames.Sub, customer.CustomerId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, customer.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var expirationMinutes = isRemember 
            ? _jwtSettings.ExpirationInMinutes * 24 
            : _jwtSettings.ExpirationInMinutes;

        var token = new JwtSecurityToken(
            issuer: "https://dev-crm-merchant-api.diadiem.vn", // From user example
            audience: "tranvuong MJ", // From user example
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
