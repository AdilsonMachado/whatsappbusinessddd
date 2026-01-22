using WhatsAppBusiness.Domain.Common;

namespace WhatsAppBusiness.Domain.Entities;

/// <summary>
/// Represents an operator who can handle conversations
/// </summary>
public class Operator : Entity
{
    /// <summary>
    /// Gets the operator's name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the operator's email
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the operator's password hash
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Gets whether the operator is currently online
    /// </summary>
    public bool IsOnline { get; private set; }

    /// <summary>
    /// Gets the date and time when the operator was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the number of current active conversations assigned to this operator
    /// </summary>
    public int CurrentConversations { get; private set; }

    private Operator()
    {
        Name = null!;
        Email = null!;
        PasswordHash = null!;
    }

    private Operator(Guid id, string name, string email, string passwordHash, DateTime createdAt)
        : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        IsOnline = false;
        CreatedAt = createdAt;
        CurrentConversations = 0;
    }

    /// <summary>
    /// Creates a new operator
    /// </summary>
    /// <param name="name">Operator's name</param>
    /// <param name="email">Operator's email</param>
    /// <param name="passwordHash">Hashed password</param>
    /// <returns>A new Operator instance</returns>
    public static Operator Create(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Operator name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Operator email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        return new Operator(Guid.NewGuid(), name, email, passwordHash, DateTime.UtcNow);
    }

    /// <summary>
    /// Sets the operator's online status
    /// </summary>
    public void SetOnlineStatus(bool isOnline)
    {
        IsOnline = isOnline;
    }

    /// <summary>
    /// Increments the current conversations count
    /// </summary>
    public void AssignConversation()
    {
        CurrentConversations++;
    }

    /// <summary>
    /// Decrements the current conversations count
    /// </summary>
    public void UnassignConversation()
    {
        if (CurrentConversations > 0)
            CurrentConversations--;
    }

    /// <summary>
    /// Updates the operator's password hash
    /// </summary>
    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Updates the operator's name
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Operator name cannot be empty", nameof(name));

        Name = name;
    }

    /// <summary>
    /// Updates the operator's email
    /// </summary>
    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Operator email cannot be empty", nameof(email));

        Email = email;
    }
}
