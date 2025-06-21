using Microsoft.AspNetCore.Mvc;

namespace ElectionsAppApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Root endpoint accessed");
        
        return Ok(new
        {
            message = "Elections App API is running",
            timestamp = DateTime.UtcNow,
            service = "ElectionsAppApi",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            port = Environment.GetEnvironmentVariable("PORT") ?? "Unknown"
        });
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
} 