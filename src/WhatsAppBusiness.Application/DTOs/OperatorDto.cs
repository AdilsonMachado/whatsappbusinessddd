namespace WhatsAppBusiness.Application.DTOs;

/// <summary>
/// Data transfer object for Operator entity
/// </summary>
public class OperatorDto
{
    /// <summary>
    /// Gets or sets the operator ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the operator's name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operator's email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the operator is online
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Gets or sets when the operator was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of current active conversations
    /// </summary>
    public int CurrentConversations { get; set; }
}
