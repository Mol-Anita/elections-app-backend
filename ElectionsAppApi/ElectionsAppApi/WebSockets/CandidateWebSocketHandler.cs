using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ElectionsAppApi.Models;
using ElectionsAppApi.Services;

namespace ElectionsAppApi.WebSockets;

public class CandidateWebSocketHandler
{
    private readonly ConcurrentDictionary<string, WebSocket> _clients = new();
    private readonly ConcurrentDictionary<string, bool> _clientGenerationStatus = new();
    private readonly ICandidateService _candidateService;
    private readonly CandidateGenerator _candidateGenerator;
    private readonly ILogger<CandidateWebSocketHandler> _logger;

    public CandidateWebSocketHandler(ICandidateService candidateService, ILogger<CandidateWebSocketHandler> logger)
    {
        _candidateService = candidateService;
        _logger = logger;
        _candidateGenerator = new CandidateGenerator();
        
        // Start the background generation task
        _ = Task.Run(GenerateCandidatesAsync);
    }

    public async Task HandleAsync(WebSocket webSocket)
    {
        var clientId = Guid.NewGuid().ToString();
        _clients.TryAdd(clientId, webSocket);
        _clientGenerationStatus.TryAdd(clientId, false);
        
        _logger.LogInformation($"Client {clientId} connected. Total clients: {_clients.Count}");

        try
        {
            var buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogInformation($"Raw message from client {clientId}: '{message}' (length: {message.Length})");
                    await HandleMessageAsync(clientId, message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation($"Client {clientId} sent close message");
                    break;
                }
                else
                {
                    _logger.LogWarning($"Client {clientId} sent unexpected message type: {result.MessageType}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling WebSocket for client {clientId}");
        }
        finally
        {
            _clients.TryRemove(clientId, out _);
            _clientGenerationStatus.TryRemove(clientId, out _);
            _logger.LogInformation($"Client {clientId} disconnected. Total clients: {_clients.Count}");
        }
    }

    private async Task HandleMessageAsync(string clientId, string message)
    {
        try
        {
            // Log the raw message for debugging
            _logger.LogInformation($"🔍 Processing message from client {clientId}: '{message}'");
            
            // Check for empty or null message
            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning($"Client {clientId} sent empty message");
                await SendToClientAsync(clientId, new { type = "error", message = "Empty message received" });
                return;
            }

            // Try to deserialize the message
            WebSocketCommand? command;
            try
            {
                command = JsonSerializer.Deserialize<WebSocketCommand>(message);
                _logger.LogInformation($"Deserialized command: Type='{command?.Type}', Raw='{message}'");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Client {clientId} sent invalid JSON: '{message}'");
                await SendToClientAsync(clientId, new { type = "error", message = "Invalid JSON format" });
                return;
            }

            // Check if command is null or has null/empty type
            if (command == null)
            {
                _logger.LogWarning($"Client {clientId} sent null command object");
                await SendToClientAsync(clientId, new { type = "error", message = "Null command object" });
                return;
            }

            if (string.IsNullOrWhiteSpace(command.Type))
            {
                _logger.LogWarning($"Client {clientId} sent command with null/empty type: '{message}'");
                await SendToClientAsync(clientId, new { type = "error", message = "Command type is null or empty" });
                return;
            }

            // Handle the command
            var commandType = command.Type.ToLower().Trim();
            _logger.LogInformation($"Processing command type: '{commandType}' from client {clientId}");
            
            switch (commandType)
            {
                case "start":
                    _clientGenerationStatus[clientId] = true;
                    await SendToClientAsync(clientId, new { type = "status", message = "Generation started" });
                    _logger.LogInformation($"Generation started for client {clientId}");
                    break;
                    
                case "stop":
                    _clientGenerationStatus[clientId] = false;
                    await SendToClientAsync(clientId, new { type = "status", message = "Generation stopped" });
                    _logger.LogInformation($"Generation stopped for client {clientId}");
                    break;
                    
                default:
                    _logger.LogWarning($"Unknown command type '{commandType}' from client {clientId}. Raw message: '{message}'");
                    await SendToClientAsync(clientId, new { type = "error", message = $"Unknown command: '{commandType}'" });
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling message from client {clientId}: '{message}'");
            await SendToClientAsync(clientId, new { type = "error", message = "Internal server error processing message" });
        }
    }

    private async Task GenerateCandidatesAsync()
    {
        while (true)
        {
            try
            {
                // Check if any client wants generation
                var activeClients = _clientGenerationStatus.Where(kvp => kvp.Value).ToList();
                
                if (activeClients.Any())
                {
                    var candidate = _candidateGenerator.GenerateCandidate();
                    
                    // Save to database
                    var savedCandidate = await _candidateService.CreateCandidateAsync(candidate);
                    
                    // Send to all active clients
                    var message = new { type = "candidate", data = savedCandidate };
                    var tasks = activeClients.Select(client => SendToClientAsync(client.Key, message));
                    await Task.WhenAll(tasks);
                    
                    _logger.LogInformation($"Generated and sent candidate {savedCandidate.Id} to {activeClients.Count} clients");
                }
                
                // Wait before next generation
                await Task.Delay(2000); // 2 seconds
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in candidate generation loop");
                await Task.Delay(5000); // Wait longer on error
            }
        }
    }

    private async Task SendToClientAsync(string clientId, object message)
    {
        if (_clients.TryGetValue(clientId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.LogDebug($"Sent message to client {clientId}: {json}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to client {clientId}");
            }
        }
        else
        {
            _logger.LogWarning($"Cannot send message to client {clientId}: WebSocket not found or not open");
        }
    }
}

public class WebSocketCommand
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
} 