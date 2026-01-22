using MediatR;
using WhatsAppBusiness.Application.Common;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Command to send a message
/// </summary>
public class SendMessageCommand : IRequest<Result>
{
    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the message text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the message is from AI
    /// </summary>
    public bool IsFromAI { get; set; }

    /// <summary>
    /// Gets or sets the operator ID (if sent by operator)
    /// </summary>
    public Guid? OperatorId { get; set; }
}
