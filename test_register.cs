using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

class Program {
    static async Task Main() {
        var client = new HttpClient();
        var json = "{\"first_name\": \"Nguyen\", \"last_name\": \"Van A\", \"merchant_name\": \"Test CRM Gateway 999\", \"phone\": \"0912345679\", \"email\": \"test999@bi-crm.vn\", \"password\": \"password123\", \"merchant_url\": \"test-crm-999\"}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("https://crm-gateway-v1-c7wqwyi1.uc.gateway.dev/api/register/insert-merchant", content);
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"StatusCode: {response.StatusCode}");
        Console.WriteLine($"Response: {result}");
    }
}
