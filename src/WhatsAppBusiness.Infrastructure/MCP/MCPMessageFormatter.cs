using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Infrastructure.MCP;

/// <summary>
/// Formats messages for MCP server
/// </summary>
public class MCPMessageFormatter
{
    /// <summary>
    /// Formats conversation messages for MCP context
    /// </summary>
    /// <param name="messages">List of messages</param>
    /// <param name="maxMessages">Maximum number of messages to include</param>
    /// <returns>Formatted conversation context</returns>
    public IEnumerable<(string role, string content)> FormatConversationContext(
        IEnumerable<Message> messages,
        int maxMessages = 10)
    {
        var context = new List<(string role, string content)>();

        var orderedMessages = messages
            .OrderByDescending(m => m.SentAt)
            .Take(maxMessages)
            .OrderBy(m => m.SentAt);

        foreach (var message in orderedMessages)
        {
            var role = message.Direction == MessageDirection.Incoming ? "user" : "assistant";
            context.Add((role, message.Content.Value));
        }

        return context;
    }

    /// <summary>
    /// Formats a single message for MCP
    /// </summary>
    /// <param name="message">Message to format</param>
    /// <returns>Formatted message tuple</returns>
    public (string role, string content) FormatMessage(Message message)
    {
        var role = message.Direction == MessageDirection.Incoming ? "user" : "assistant";
        return (role, message.Content.Value);
    }

    /// <summary>
    /// Adds system prompt to conversation context
    /// </summary>
    /// <param name="systemPrompt">System prompt text</param>
    /// <param name="context">Existing conversation context</param>
    /// <returns>Context with system prompt prepended</returns>
    public IEnumerable<(string role, string content)> AddSystemPrompt(
        string systemPrompt,
        IEnumerable<(string role, string content)> context)
    {
        var result = new List<(string role, string content)>
        {
            ("system", systemPrompt)
        };

        result.AddRange(context);
        return result;
    }

    /// <summary>
    /// Creates a default system prompt for customer service
    /// </summary>
    /// <returns>System prompt text</returns>
    public string GetDefaultSystemPrompt()
    {
        return @"You are a helpful customer service assistant for a WhatsApp Business account.
Your role is to assist customers with their inquiries in a friendly and professional manner.
If you cannot help with a specific request, politely inform the customer that you will transfer them to a human operator.
Keep your responses concise and relevant to the customer's questions.";
    }
}
