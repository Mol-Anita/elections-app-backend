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

// Enable WebSockets
app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

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

app.Run();
