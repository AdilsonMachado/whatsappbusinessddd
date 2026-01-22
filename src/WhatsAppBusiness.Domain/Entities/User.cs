using WhatsAppBusiness.Domain.Common;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User : Entity
{
    /// <summary>
    /// Gets the user's name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the user's phone number
    /// </summary>
    public PhoneNumber PhoneNumber { get; private set; }

    /// <summary>
    /// Gets whether the user is authenticated
    /// </summary>
    public bool IsAuthenticated { get; private set; }

    /// <summary>
    /// Gets the date and time when the user was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time of the last interaction
    /// </summary>
    public DateTime? LastInteractionAt { get; private set; }

    private User()
    {
        Name = null!;
        PhoneNumber = null!;
    }

    private User(Guid id, string name, PhoneNumber phoneNumber, bool isAuthenticated, DateTime createdAt)
        : base(id)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        IsAuthenticated = isAuthenticated;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="name">User's name</param>
    /// <param name="phoneNumber">User's phone number</param>
    /// <returns>A new User instance</returns>
    public static User Create(string name, PhoneNumber phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be empty", nameof(name));

        return new User(Guid.NewGuid(), name, phoneNumber, false, DateTime.UtcNow);
    }

    /// <summary>
    /// Marks the user as authenticated
    /// </summary>
    public void Authenticate()
    {
        IsAuthenticated = true;
        UpdateLastInteraction();
    }

    /// <summary>
    /// Updates the name of the user
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be empty", nameof(name));

        Name = name;
    }

    /// <summary>
    /// Updates the last interaction timestamp
    /// </summary>
    public void UpdateLastInteraction()
    {
        LastInteractionAt = DateTime.UtcNow;
    }
}
