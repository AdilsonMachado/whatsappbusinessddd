using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace WhatsAppBusiness.Infrastructure.Security;

/// <summary>
/// Service for encrypting and decrypting data using ASP.NET Core Data Protection
/// </summary>
public class EncryptionService
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly ILogger<EncryptionService> _logger;
    private const string Purpose = "WhatsAppBusiness.Encryption";

    public EncryptionService(
        IDataProtectionProvider dataProtectionProvider,
        ILogger<EncryptionService> logger)
    {
        _dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Encrypts a string value
    /// </summary>
    /// <param name="plainText">Plain text to encrypt</param>
    /// <returns>Encrypted string</returns>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            throw new ArgumentException("Plain text cannot be empty", nameof(plainText));

        try
        {
            var protector = _dataProtectionProvider.CreateProtector(Purpose);
            var encrypted = protector.Protect(plainText);

            _logger.LogDebug("Data encrypted successfully");
            return encrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting data");
            throw;
        }
    }

    /// <summary>
    /// Decrypts a string value
    /// </summary>
    /// <param name="encryptedText">Encrypted text to decrypt</param>
    /// <returns>Decrypted string</returns>
    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            throw new ArgumentException("Encrypted text cannot be empty", nameof(encryptedText));

        try
        {
            var protector = _dataProtectionProvider.CreateProtector(Purpose);
            var decrypted = protector.Unprotect(encryptedText);

            _logger.LogDebug("Data decrypted successfully");
            return decrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting data");
            throw;
        }
    }

    /// <summary>
    /// Encrypts a string value with a specific purpose
    /// </summary>
    /// <param name="plainText">Plain text to encrypt</param>
    /// <param name="purpose">Purpose for encryption (creates isolated protector)</param>
    /// <returns>Encrypted string</returns>
    public string EncryptWithPurpose(string plainText, string purpose)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            throw new ArgumentException("Plain text cannot be empty", nameof(plainText));

        if (string.IsNullOrWhiteSpace(purpose))
            throw new ArgumentException("Purpose cannot be empty", nameof(purpose));

        try
        {
            var protector = _dataProtectionProvider.CreateProtector(purpose);
            var encrypted = protector.Protect(plainText);

            _logger.LogDebug("Data encrypted successfully with purpose: {Purpose}", purpose);
            return encrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting data with purpose: {Purpose}", purpose);
            throw;
        }
    }

    /// <summary>
    /// Decrypts a string value with a specific purpose
    /// </summary>
    /// <param name="encryptedText">Encrypted text to decrypt</param>
    /// <param name="purpose">Purpose for decryption (must match encryption purpose)</param>
    /// <returns>Decrypted string</returns>
    public string DecryptWithPurpose(string encryptedText, string purpose)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            throw new ArgumentException("Encrypted text cannot be empty", nameof(encryptedText));

        if (string.IsNullOrWhiteSpace(purpose))
            throw new ArgumentException("Purpose cannot be empty", nameof(purpose));

        try
        {
            var protector = _dataProtectionProvider.CreateProtector(purpose);
            var decrypted = protector.Unprotect(encryptedText);

            _logger.LogDebug("Data decrypted successfully with purpose: {Purpose}", purpose);
            return decrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting data with purpose: {Purpose}", purpose);
            throw;
        }
    }
}
