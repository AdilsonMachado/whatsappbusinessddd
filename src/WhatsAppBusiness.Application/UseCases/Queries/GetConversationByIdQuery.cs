using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Query to get a conversation by ID
/// </summary>
public class GetConversationByIdQuery : IRequest<Result<ConversationDto>>
{
    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets whether to include messages
    /// </summary>
    public bool IncludeMessages { get; set; } = true;

    public GetConversationByIdQuery(Guid conversationId, bool includeMessages = true)
    {
        ConversationId = conversationId;
        IncludeMessages = includeMessages;
    }
}
