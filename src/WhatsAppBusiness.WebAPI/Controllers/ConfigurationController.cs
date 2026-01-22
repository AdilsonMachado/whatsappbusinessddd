using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WhatsAppBusiness.WebAPI.Controllers;

/// <summary>
/// Controller for managing application configuration
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConfigurationController> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the ConfigurationController
    /// </summary>
    public ConfigurationController(
        IMediator mediator, 
        ILogger<ConfigurationController> logger,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Get non-sensitive configuration settings
    /// </summary>
    /// <returns>Configuration settings</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ConfigurationSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSettings()
    {
        _logger.LogInformation("Getting configuration settings");
        
        var settings = new ConfigurationSettingsResponse
        {
            WhatsAppApiBaseUrl = _configuration["WhatsApp:ApiBaseUrl"] ?? string.Empty,
            McpServerUrl = _configuration["MCP:ServerUrl"] ?? string.Empty,
            // Note: We do not expose sensitive fields like tokens, secrets, etc.
        };

        return Ok(settings);
    }

    /// <summary>
    /// Update non-sensitive configuration settings
    /// </summary>
    /// <param name="request">The settings to update</param>
    /// <returns>Success result</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateConfigurationRequest request)
    {
        _logger.LogInformation("Updating configuration settings");
        
        // Note: We need to add an UpdateConfigurationCommand in the future
        // For now, we'll return a NotImplemented response
        return StatusCode(501, new { error = "Update configuration endpoint not yet implemented" });
    }
}

/// <summary>
/// Response model for configuration settings
/// </summary>
public class ConfigurationSettingsResponse
{
    /// <summary>
    /// WhatsApp API base URL
    /// </summary>
    public string WhatsAppApiBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// MCP server URL
    /// </summary>
    public string McpServerUrl { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating configuration
/// </summary>
public class UpdateConfigurationRequest
{
    /// <summary>
    /// WhatsApp API base URL
    /// </summary>
    public string? WhatsAppApiBaseUrl { get; set; }

    /// <summary>
    /// MCP server URL
    /// </summary>
    public string? McpServerUrl { get; set; }
}
