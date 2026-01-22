using System.Text.Json;
using Microsoft.Extensions.Logging;
using WhatsAppBusiness.Infrastructure.Configuration.Models;

namespace WhatsAppBusiness.Infrastructure.Configuration;

/// <summary>
/// Manages reading and writing of configuration file
/// </summary>
public class ConfigurationManager
{
    private readonly string _configFilePath;
    private readonly ILogger<ConfigurationManager> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigurationManager(string configFilePath, ILogger<ConfigurationManager> logger)
    {
        _configFilePath = configFilePath ?? throw new ArgumentNullException(nameof(configFilePath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    /// <summary>
    /// Reads configuration from file
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Application configuration</returns>
    public async Task<AppConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                _logger.LogWarning("Configuration file not found at {Path}, creating default configuration", _configFilePath);
                var defaultConfig = new AppConfiguration();
                await WriteConfigurationAsync(defaultConfig, cancellationToken);
                return defaultConfig;
            }

            var json = await File.ReadAllTextAsync(_configFilePath, cancellationToken);
            var config = JsonSerializer.Deserialize<AppConfiguration>(json, _jsonOptions);

            if (config == null)
            {
                _logger.LogWarning("Failed to deserialize configuration, returning default");
                return new AppConfiguration();
            }

            _logger.LogInformation("Configuration loaded from {Path}", _configFilePath);
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading configuration file from {Path}", _configFilePath);
            throw;
        }
    }

    /// <summary>
    /// Writes configuration to file
    /// </summary>
    /// <param name="configuration">Configuration to write</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task WriteConfigurationAsync(AppConfiguration configuration, CancellationToken cancellationToken = default)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        try
        {
            var directory = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(configuration, _jsonOptions);
            await File.WriteAllTextAsync(_configFilePath, json, cancellationToken);

            _logger.LogInformation("Configuration written to {Path}", _configFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing configuration file to {Path}", _configFilePath);
            throw;
        }
    }

    /// <summary>
    /// Updates specific section of configuration
    /// </summary>
    /// <param name="updateAction">Action to update configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task UpdateConfigurationAsync(
        Action<AppConfiguration> updateAction,
        CancellationToken cancellationToken = default)
    {
        if (updateAction == null)
            throw new ArgumentNullException(nameof(updateAction));

        var config = await ReadConfigurationAsync(cancellationToken);
        updateAction(config);
        await WriteConfigurationAsync(config, cancellationToken);
    }
}
