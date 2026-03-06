using Microsoft.AspNetCore.Mvc;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.BackgroundWorker.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestWorkerController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<TestWorkerController> _logger;

        public TestWorkerController(IBackgroundTaskQueue taskQueue, ILogger<TestWorkerController> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        [HttpPost("test-queue")]
        public async Task<IActionResult> TestQueue([FromQuery] string message)
        {
            await _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                _logger.LogInformation("Processing background task: {Message}", message);
                await Task.Delay(5000, token); // Simulate work
                _logger.LogInformation("Completed background task: {Message}", message);
            });

            return Ok(new { Message = "Task queued successfully", Details = message });
        }
    }
}
