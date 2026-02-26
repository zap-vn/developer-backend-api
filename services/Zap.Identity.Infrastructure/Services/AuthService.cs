using BCrypt.Net;
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
            request.UserName ?? string.Empty, 
            request.MerchantName ?? string.Empty);


        if (customer == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid credentials. Customer not found."
            };
        }

        // Verify password
        var isLegacyMatch = customer.Password == HashPassword(request.Password);
        var isBCryptMatch = false;
        
        try 
        {
            // BCrypt hashes start with $2a$, $2b$ or $2y$
            if (customer.Password.StartsWith("$2"))
            {
                isBCryptMatch = BCrypt.Net.BCrypt.Verify(request.Password, customer.Password);
            }
        }
        catch { /* Not a BCrypt hash */ }

        if (!isLegacyMatch && !isBCryptMatch)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid credentials. Incorrect password."
            };
        }

        // Check if customer is active
        if (customer.Visible != 1)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Account is not active."
            };
        }


        // Generate JWT token
        var token = GenerateJwtToken(customer, request.IsRemember);

        var fullName = $"{customer.FirstName} {customer.LastName}".Trim();
        var acronym = GenerateAcronym(fullName);

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            MerchantName = customer.MerchantName,
            AccessToken = token,
            Acronym = acronym,
            Avatar = customer.Url,
            ExpiresIn = (request.IsRemember ? _jwtSettings.ExpirationInMinutes * 24 : _jwtSettings.ExpirationInMinutes) * 60,
            FullName = fullName,
            RefreshToken = Guid.NewGuid().ToString(),
            Role = "Owner (Super Admin)",
            UserGuid = $"Customer/{customer.CustomerId}"
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var customer = new Customer
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MerchantName = request.MerchantName,
            Phone = request.Phone ?? string.Empty,
            CustomerStatusId = 1,
            Visible = 1,
            CreateDate = DateTime.UtcNow.ToString("O"),
            Language = "vi",
            DateFormat = "dd/MM/yyyy",
            TimeFormat = "HH:mm"
        };


        await _customerRepository.CreateAsync(customer);

        // Auto login after registration
        return await LoginAsync(new LoginRequest
        {
            UserName = request.Email,
            Password = request.Password,
            MerchantName = request.MerchantName
        });
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

    private string GenerateAcronym(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "??";

        var words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length >= 2)
        {
            // Multiple words: take first character of first 2 words
            return $"{words[0][0]}{words[1][0]}".ToUpper();
        }

        // Single word: take first 2 characters
        return fullName.Length >= 2
            ? fullName.Substring(0, 2).ToUpper()
            : fullName.ToUpper().PadRight(2, 'X');
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
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
