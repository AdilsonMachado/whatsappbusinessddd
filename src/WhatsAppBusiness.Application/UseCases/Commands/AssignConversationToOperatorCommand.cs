using MediatR;
using WhatsAppBusiness.Application.Common;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Command to assign a conversation to an operator
/// </summary>
public class AssignConversationToOperatorCommand : IRequest<Result>
{
    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the operator ID
    /// </summary>
    public Guid OperatorId { get; set; }
}
