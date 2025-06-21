using Microsoft.AspNetCore.Mvc;

namespace ElectionsAppApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Health check requested");
        
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "ElectionsAppApi",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        });
    }

    [HttpGet("ready")]
    public IActionResult Ready()
    {
        // Add any readiness checks here (database connectivity, etc.)
        return Ok(new
        {
            status = "ready",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        // Simple liveness check
        return Ok(new
        {
            status = "alive",
            timestamp = DateTime.UtcNow
        });
    }
} 