using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Query to get conversation history (messages)
/// </summary>
public class GetConversationHistoryQuery : IRequest<Result<List<MessageDto>>>
{
    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of messages to retrieve
    /// </summary>
    public int? Limit { get; set; }

    public GetConversationHistoryQuery(Guid conversationId, int? limit = null)
    {
        ConversationId = conversationId;
        Limit = limit;
    }
}
