using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace WhatsAppBusiness.Infrastructure.Security;

/// <summary>
/// Manages encryption keys and secrets
/// </summary>
public class SecretManager
{
    private readonly ILogger<SecretManager> _logger;
    private readonly Dictionary<string, string> _secrets;

    public SecretManager(ILogger<SecretManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _secrets = new Dictionary<string, string>();
    }

    /// <summary>
    /// Generates a cryptographically secure random key
    /// </summary>
    /// <param name="keySize">Size of the key in bytes (default: 32)</param>
    /// <returns>Base64-encoded random key</returns>
    public string GenerateRandomKey(int keySize = 32)
    {
        if (keySize <= 0)
            throw new ArgumentException("Key size must be greater than 0", nameof(keySize));

        try
        {
            var keyBytes = new byte[keySize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(keyBytes);

            var key = Convert.ToBase64String(keyBytes);
            _logger.LogDebug("Generated random key with size {KeySize} bytes", keySize);

            return key;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating random key");
            throw;
        }
    }

    /// <summary>
    /// Stores a secret in memory
    /// </summary>
    /// <param name="key">Secret key identifier</param>
    /// <param name="value">Secret value</param>
    public void SetSecret(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));

        _secrets[key] = value;
        _logger.LogDebug("Secret stored with key: {Key}", key);
    }

    /// <summary>
    /// Retrieves a secret from memory
    /// </summary>
    /// <param name="key">Secret key identifier</param>
    /// <returns>Secret value if found, null otherwise</returns>
    public string? GetSecret(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        if (_secrets.TryGetValue(key, out var value))
        {
            _logger.LogDebug("Secret retrieved with key: {Key}", key);
            return value;
        }

        _logger.LogWarning("Secret not found with key: {Key}", key);
        return null;
    }

    /// <summary>
    /// Removes a secret from memory
    /// </summary>
    /// <param name="key">Secret key identifier</param>
    /// <returns>True if secret was removed, false if not found</returns>
    public bool RemoveSecret(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        var removed = _secrets.Remove(key);
        if (removed)
        {
            _logger.LogDebug("Secret removed with key: {Key}", key);
        }
        else
        {
            _logger.LogWarning("Failed to remove secret with key: {Key}", key);
        }

        return removed;
    }

    /// <summary>
    /// Checks if a secret exists
    /// </summary>
    /// <param name="key">Secret key identifier</param>
    /// <returns>True if secret exists, false otherwise</returns>
    public bool HasSecret(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        return _secrets.ContainsKey(key);
    }

    /// <summary>
    /// Clears all secrets from memory
    /// </summary>
    public void ClearAllSecrets()
    {
        var count = _secrets.Count;
        _secrets.Clear();
        _logger.LogInformation("Cleared {Count} secrets from memory", count);
    }

    /// <summary>
    /// Generates a secure webhook token
    /// </summary>
    /// <returns>Random webhook token</returns>
    public string GenerateWebhookToken()
    {
        return GenerateRandomKey(32);
    }
}
