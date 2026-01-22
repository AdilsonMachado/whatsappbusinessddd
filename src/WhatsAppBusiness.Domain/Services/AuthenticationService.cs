using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Services;

/// <summary>
/// Domain service for user authentication logic
/// </summary>
public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IConversationRepository _conversationRepository;

    public AuthenticationService(IUserRepository userRepository, IConversationRepository conversationRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));
    }

    /// <summary>
    /// Authenticates a user by phone number
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <param name="userName">The user's name for registration if not exists</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The authenticated user and their active conversation</returns>
    public async Task<(User User, Conversation Conversation)> AuthenticateUserAsync(
        PhoneNumber phoneNumber,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

        if (user == null)
        {
            user = User.Create(userName, phoneNumber);
            await _userRepository.AddAsync(user, cancellationToken);
        }

        if (!user.IsAuthenticated)
        {
            user.Authenticate();
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
        else
        {
            user.UpdateLastInteraction();
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        var conversation = await _conversationRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

        if (conversation == null)
        {
            conversation = Conversation.Create(user.Id, phoneNumber);
            conversation.MarkAsAuthenticated();
            await _conversationRepository.AddAsync(conversation, cancellationToken);
        }
        else
        {
            conversation.MarkAsAuthenticated();
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        }

        return (user, conversation);
    }

    /// <summary>
    /// Verifies if a user is authenticated
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if authenticated, false otherwise</returns>
    public async Task<bool> IsUserAuthenticatedAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);
        return user?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Gets or creates a user by phone number
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <param name="defaultName">Default name if user doesn't exist</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user</returns>
    public async Task<User> GetOrCreateUserAsync(
        PhoneNumber phoneNumber,
        string defaultName,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

        if (user == null)
        {
            user = User.Create(defaultName, phoneNumber);
            await _userRepository.AddAsync(user, cancellationToken);
        }

        return user;
    }
}
