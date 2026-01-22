using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Application.DTOs;

/// <summary>
/// Data transfer object for Conversation entity
/// </summary>
public class ConversationDto
{
    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the conversation status
    /// </summary>
    public ConversationStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when the conversation was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the conversation was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the assigned operator ID
    /// </summary>
    public Guid? AssignedOperatorId { get; set; }

    /// <summary>
    /// Gets or sets whether the conversation is with AI
    /// </summary>
    public bool IsWithAI { get; set; }

    /// <summary>
    /// Gets or sets the user information
    /// </summary>
    public UserDto? User { get; set; }

    /// <summary>
    /// Gets or sets the assigned operator information
    /// </summary>
    public OperatorDto? AssignedOperator { get; set; }

    /// <summary>
    /// Gets or sets the messages in the conversation
    /// </summary>
    public List<MessageDto>? Messages { get; set; }
}
