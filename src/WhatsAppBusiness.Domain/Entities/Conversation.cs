using WhatsAppBusiness.Domain.Common;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Entities;

/// <summary>
/// Represents a conversation between a user and the system
/// </summary>
public class Conversation : Entity
{
    /// <summary>
    /// Gets the user ID associated with this conversation
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the phone number of the conversation
    /// </summary>
    public PhoneNumber PhoneNumber { get; private set; }

    /// <summary>
    /// Gets the current status of the conversation
    /// </summary>
    public ConversationStatus Status { get; private set; }

    /// <summary>
    /// Gets the date and time when the conversation was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the conversation was last updated
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the ID of the operator assigned to this conversation, if any
    /// </summary>
    public Guid? AssignedOperatorId { get; private set; }

    /// <summary>
    /// Gets whether the conversation is currently being handled by AI
    /// </summary>
    public bool IsWithAI { get; private set; }

    private Conversation()
    {
        PhoneNumber = null!;
    }

    private Conversation(Guid id, Guid userId, PhoneNumber phoneNumber, ConversationStatus status, DateTime createdAt)
        : base(id)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        IsWithAI = status == ConversationStatus.WithAI;
    }

    /// <summary>
    /// Creates a new conversation
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>A new Conversation instance</returns>
    public static Conversation Create(Guid userId, PhoneNumber phoneNumber)
    {
        return new Conversation(Guid.NewGuid(), userId, phoneNumber, ConversationStatus.WaitingAuthentication, DateTime.UtcNow);
    }

    /// <summary>
    /// Starts AI handling of the conversation
    /// </summary>
    public void StartAIHandling()
    {
        if (Status == ConversationStatus.Closed)
            throw new InvalidOperationException("Cannot start AI handling on a closed conversation");

        Status = ConversationStatus.WithAI;
        IsWithAI = true;
        AssignedOperatorId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns the conversation to an operator
    /// </summary>
    /// <param name="operatorId">The operator ID</param>
    public void AssignToOperator(Guid operatorId)
    {
        if (Status == ConversationStatus.Closed)
            throw new InvalidOperationException("Cannot assign a closed conversation to an operator");

        Status = ConversationStatus.WithOperator;
        AssignedOperatorId = operatorId;
        IsWithAI = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Closes the conversation
    /// </summary>
    public void Close()
    {
        Status = ConversationStatus.Closed;
        IsWithAI = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the conversation after user authentication
    /// </summary>
    public void MarkAsAuthenticated()
    {
        if (Status == ConversationStatus.WaitingAuthentication)
        {
            Status = ConversationStatus.WithAI;
            IsWithAI = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Updates the last updated timestamp
    /// </summary>
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
