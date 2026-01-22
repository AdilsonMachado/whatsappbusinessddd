using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using WhatsAppBusiness.Infrastructure.Configuration.Models;

namespace WhatsAppBusiness.Infrastructure.Configuration;

/// <summary>
/// Service for encrypting and decrypting sensitive configuration data
/// </summary>
public class SecureConfigurationService
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly ILogger<SecureConfigurationService> _logger;
    private readonly IDataProtector _protector;

    public SecureConfigurationService(
        IDataProtectionProvider dataProtectionProvider,
        ILogger<SecureConfigurationService> logger)
    {
        _dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _protector = _dataProtectionProvider.CreateProtector("WhatsAppBusiness.Configuration");
    }

    /// <summary>
    /// Encrypts sensitive data in configuration
    /// </summary>
    /// <param name="configuration">Configuration to encrypt</param>
    /// <returns>Configuration with encrypted values</returns>
    public AppConfiguration EncryptSensitiveData(AppConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        try
        {
            var encrypted = new AppConfiguration
            {
                WhatsApp = new WhatsAppSettings
                {
                    PhoneNumberId = configuration.WhatsApp.PhoneNumberId,
                    AccessToken = EncryptValue(configuration.WhatsApp.AccessToken),
                    VerifyToken = EncryptValue(configuration.WhatsApp.VerifyToken),
                    BusinessAccountId = configuration.WhatsApp.BusinessAccountId,
                    AppSecret = EncryptValue(configuration.WhatsApp.AppSecret),
                    ApiBaseUrl = configuration.WhatsApp.ApiBaseUrl
                },
                Database = new DatabaseSettings
                {
                    ConnectionString = EncryptValue(configuration.Database.ConnectionString)
                },
                MCP = new MCPSettings
                {
                    ServerUrl = configuration.MCP.ServerUrl,
                    ApiKey = EncryptValue(configuration.MCP.ApiKey)
                }
            };

            _logger.LogDebug("Sensitive configuration data encrypted");
            return encrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting sensitive configuration data");
            throw;
        }
    }

    /// <summary>
    /// Decrypts sensitive data in configuration
    /// </summary>
    /// <param name="configuration">Configuration to decrypt</param>
    /// <returns>Configuration with decrypted values</returns>
    public AppConfiguration DecryptSensitiveData(AppConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        try
        {
            var decrypted = new AppConfiguration
            {
                WhatsApp = new WhatsAppSettings
                {
                    PhoneNumberId = configuration.WhatsApp.PhoneNumberId,
                    AccessToken = DecryptValue(configuration.WhatsApp.AccessToken),
                    VerifyToken = DecryptValue(configuration.WhatsApp.VerifyToken),
                    BusinessAccountId = configuration.WhatsApp.BusinessAccountId,
                    AppSecret = DecryptValue(configuration.WhatsApp.AppSecret),
                    ApiBaseUrl = configuration.WhatsApp.ApiBaseUrl
                },
                Database = new DatabaseSettings
                {
                    ConnectionString = DecryptValue(configuration.Database.ConnectionString)
                },
                MCP = new MCPSettings
                {
                    ServerUrl = configuration.MCP.ServerUrl,
                    ApiKey = DecryptValue(configuration.MCP.ApiKey)
                }
            };

            _logger.LogDebug("Sensitive configuration data decrypted");
            return decrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting sensitive configuration data");
            throw;
        }
    }

    private string EncryptValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return _protector.Protect(value);
    }

    private string DecryptValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        try
        {
            return _protector.Unprotect(value);
        }
        catch
        {
            // If decryption fails, assume value is not encrypted
            _logger.LogWarning("Failed to decrypt value, assuming it's not encrypted");
            return value;
        }
    }
}
