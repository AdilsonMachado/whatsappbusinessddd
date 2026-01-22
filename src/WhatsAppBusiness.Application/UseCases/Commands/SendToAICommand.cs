using MediatR;
using WhatsAppBusiness.Application.Common;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Command to send a conversation to AI
/// </summary>
public class SendToAICommand : IRequest<Result>
{
    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }
}
