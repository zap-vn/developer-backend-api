// Test script to login and get JWT token
// Usage: 
// 1. Update the credentials below
// 2. Run: dotnet script test_login.csx
// 3. Copy the token and use it in your API requests

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

var apiUrl = "http://localhost:5271/api/Auth/login";

// UPDATE THESE CREDENTIALS
var loginRequest = new
{
    UserName = "your-email@example.com",        // Change this
    Password = "your-password",                  // Change this
    MerchantName = "your-merchant-name",        // Change this
    IsRemember = false
};

Console.WriteLine("=== Testing Login API ===");
Console.WriteLine($"API URL: {apiUrl}");
Console.WriteLine($"Username: {loginRequest.UserName}");
Console.WriteLine($"Merchant: {loginRequest.MerchantName}");
Console.WriteLine();

try
{
    using var client = new HttpClient();
    var response = await client.PostAsJsonAsync(apiUrl, loginRequest);
    
    Console.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
    Console.WriteLine();
    
    var content = await response.Content.ReadAsStringAsync();
    
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("✓ Login Successful!");
        Console.WriteLine();
        
        var jsonDoc = JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;
        
        if (root.TryGetProperty("AccessToken", out var tokenElement))
        {
            var token = tokenElement.GetString();
            Console.WriteLine("=== ACCESS TOKEN ===");
            Console.WriteLine(token);
            Console.WriteLine();
            Console.WriteLine("=== FULL RESPONSE ===");
            Console.WriteLine(JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine();
            Console.WriteLine("=== HOW TO USE ===");
            Console.WriteLine("Add this header to your requests:");
            Console.WriteLine($"Authorization: Bearer {token}");
        }
        else
        {
            Console.WriteLine("Response:");
            Console.WriteLine(content);
        }
    }
    else
    {
        Console.WriteLine("✗ Login Failed!");
        Console.WriteLine("Response:");
        Console.WriteLine(content);
    }
}
catch (Exception ex)
{
    Console.WriteLine("✗ Error occurred!");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Make sure:");
    Console.WriteLine("1. The API is running (dotnet run --project services/Zap.Identity.Api)");
    Console.WriteLine("2. The API is accessible at http://localhost:5271");
    Console.WriteLine("3. MongoDB is running and accessible");
}
