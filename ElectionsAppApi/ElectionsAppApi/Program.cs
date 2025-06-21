using ElectionsAppApi.Repositories;
using ElectionsAppApi.Services;
using ElectionsAppApi.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register candidate services
builder.Services.AddSingleton<ICandidateRepository, InMemoryCandidateRepository>();
builder.Services.AddSingleton<ICandidateService, CandidateService>();

// Register WebSocket handler
builder.Services.AddSingleton<CandidateWebSocketHandler>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Log startup information early
Console.WriteLine("=== Elections App API Starting ===");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Port: {Environment.GetEnvironmentVariable("PORT") ?? "Not set"}");
Console.WriteLine($"URLs: {string.Join(", ", builder.Configuration["ASPNETCORE_URLS"] ?? "Default")}");

// Enable WebSockets
app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Only use HTTPS redirection in development or when explicitly configured
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("FORCE_HTTPS") == "true")
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Add a simple root endpoint for health checks
app.MapGet("/", () => {
    Console.WriteLine("Root endpoint accessed");
    return new
    {
        message = "Elections App API is running",
        timestamp = DateTime.UtcNow,
        environment = app.Environment.EnvironmentName,
        port = Environment.GetEnvironmentVariable("PORT") ?? "Unknown",
        urls = builder.Configuration["ASPNETCORE_URLS"] ?? "Default"
    };
});

// Add a simple health endpoint
app.MapGet("/health", () => {
    Console.WriteLine("Health endpoint accessed");
    return new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        environment = app.Environment.EnvironmentName,
        port = Environment.GetEnvironmentVariable("PORT") ?? "Unknown"
    };
});

// Add a simple test endpoint
app.MapGet("/test", () => {
    Console.WriteLine("Test endpoint accessed");
    return "API is working!";
});

// Map WebSocket endpoint
app.Map("/ws/candidates", async context =>
{
    Console.WriteLine($"WebSocket request received: {context.Request.Path}");
    Console.WriteLine($"Is WebSocket request: {context.WebSockets.IsWebSocketRequest}");
    
    if (context.WebSockets.IsWebSocketRequest)
    {
        Console.WriteLine("Accepting WebSocket connection...");
        var handler = context.RequestServices.GetRequiredService<CandidateWebSocketHandler>();
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket accepted, starting handler...");
        await handler.HandleAsync(webSocket);
    }
    else
    {
        Console.WriteLine("Not a WebSocket request, returning 400");
        context.Response.StatusCode = 400;
    }
});

Console.WriteLine("=== Application configured, starting... ===");
Console.WriteLine($"Application started successfully at {DateTime.UtcNow}");

app.Run();
