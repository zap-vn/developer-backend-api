using System;
using System.Net.Http;
using System.Threading.Tasks;

var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

async Task TestUrl(string name, string url)
{
    Console.WriteLine($"Testing {name}: {url}");
    try
    {
        var response = await client.GetAsync(url);
        Console.WriteLine($"Status: {response.StatusCode}");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Content: {content}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    Console.WriteLine("-----------------------------------");
}

await TestUrl("Product Health", "http://localhost:56335/api/Products/health");
await TestUrl("Product List", "http://localhost:56335/api/Products/list");
await TestUrl("Gateway Health", "http://localhost:5000/health");
await TestUrl("Gateway Product List", "http://localhost:5000/api/Products/list");
