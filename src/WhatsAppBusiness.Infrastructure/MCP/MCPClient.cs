using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WhatsAppBusiness.Infrastructure.MCP;

/// <summary>
/// Client for MCP (Model Context Protocol) server communication
/// </summary>
public class MCPClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MCPClient> _logger;

    public MCPClient(HttpClient httpClient, ILogger<MCPClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends a message to AI and gets a response
    /// </summary>
    /// <param name="message">User message</param>
    /// <param name="conversationContext">Previous conversation messages for context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI response</returns>
    public async Task<string> SendMessageAsync(
        string message,
        IEnumerable<(string role, string content)>? conversationContext = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        var messages = new List<object>();

        // Add conversation context if provided
        if (conversationContext != null)
        {
            foreach (var (role, content) in conversationContext)
            {
                messages.Add(new { role, content });
            }
        }

        // Add current message
        messages.Add(new { role = "user", content = message });

        var payload = new
        {
            model = "gpt-4",
            messages,
            temperature = 0.7,
            max_tokens = 500
        };

        var json = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogDebug("Sending message to MCP server: {Message}", message);

        var response = await _httpClient.PostAsync("/v1/chat/completions", httpContent, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("MCP server request failed with status {StatusCode}: {Response}",
                response.StatusCode, responseContent);
            throw new HttpRequestException(
                $"MCP server request failed with status {response.StatusCode}: {responseContent}");
        }

        _logger.LogDebug("MCP server response: {Response}", responseContent);

        return ExtractAIResponse(responseContent);
    }

    /// <summary>
    /// Gets AI response with full conversation context
    /// </summary>
    /// <param name="conversationHistory">List of messages in the conversation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI response</returns>
    public async Task<string> GetResponseWithContextAsync(
        IEnumerable<(string role, string content)> conversationHistory,
        CancellationToken cancellationToken = default)
    {
        var messages = conversationHistory
            .Select(m => new { role = m.role, content = m.content })
            .ToList();

        if (messages.Count == 0)
            throw new ArgumentException("Conversation history cannot be empty", nameof(conversationHistory));

        var payload = new
        {
            model = "gpt-4",
            messages,
            temperature = 0.7,
            max_tokens = 500
        };

        var json = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogDebug("Sending conversation context to MCP server with {Count} messages", messages.Count);

        var response = await _httpClient.PostAsync("/v1/chat/completions", httpContent, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("MCP server request failed with status {StatusCode}: {Response}",
                response.StatusCode, responseContent);
            throw new HttpRequestException(
                $"MCP server request failed with status {response.StatusCode}: {responseContent}");
        }

        return ExtractAIResponse(responseContent);
    }

    private static string ExtractAIResponse(string response)
    {
        using var document = JsonDocument.Parse(response);
        var root = document.RootElement;

        if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
        {
            var firstChoice = choices[0];
            if (firstChoice.TryGetProperty("message", out var message))
            {
                if (message.TryGetProperty("content", out var content))
                {
                    return content.GetString() ?? "No response from AI";
                }
            }
        }

        throw new InvalidOperationException("Failed to extract AI response from MCP server response");
    }
}
