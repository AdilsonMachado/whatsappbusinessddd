namespace WhatsAppBusiness.Application.DTOs;

/// <summary>
/// Data transfer object for User entity
/// </summary>
public class UserDto
{
    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user's name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the user is authenticated
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Gets or sets when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last interaction timestamp
    /// </summary>
    public DateTime? LastInteractionAt { get; set; }
}
