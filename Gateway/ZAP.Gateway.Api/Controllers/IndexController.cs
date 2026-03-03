using Microsoft.AspNetCore.Mvc;

namespace ZAP.Gateway.Api.Controllers
{
    [ApiController]
    [Route("/")]
    public class IndexController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "ZAP Ecosystem Gateway is running",
                Version = "1.1.0",
                Environment = "Development",
                Services = new[] { "Authentication", "HR", "Customer", "Sales", "Product", "Order", "Payment", "Organization", "Report" },
                HealthCheck = "/health/auth",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
