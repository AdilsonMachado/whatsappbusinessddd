using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Query to get all active conversations
/// </summary>
public class GetActiveConversationsQuery : IRequest<Result<List<ConversationDto>>>
{
}
