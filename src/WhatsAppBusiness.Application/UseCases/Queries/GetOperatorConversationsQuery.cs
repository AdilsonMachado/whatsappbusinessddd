using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Query to get conversations assigned to an operator
/// </summary>
public class GetOperatorConversationsQuery : IRequest<Result<List<ConversationDto>>>
{
    /// <summary>
    /// Gets or sets the operator ID
    /// </summary>
    public Guid OperatorId { get; set; }

    public GetOperatorConversationsQuery(Guid operatorId)
    {
        OperatorId = operatorId;
    }
}
