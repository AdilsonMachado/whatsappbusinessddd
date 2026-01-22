using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Infrastructure.Configuration;
using WhatsAppBusiness.Infrastructure.Data;
using WhatsAppBusiness.Infrastructure.Data.Repositories;
using WhatsAppBusiness.Infrastructure.MCP;
using WhatsAppBusiness.Infrastructure.Security;
using WhatsAppBusiness.Infrastructure.WhatsApp;

namespace WhatsAppBusiness.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure services with DI container
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<WhatsAppBusinessDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string 'DefaultConnection' not found");

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            });
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IOperatorRepository, OperatorRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

        // Add Data Protection
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")))
            .SetApplicationName("WhatsAppBusiness");

        // Register security services
        services.AddSingleton<EncryptionService>();
        services.AddSingleton<SecretManager>();

        // Register configuration services
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        services.AddSingleton(sp => 
            new WhatsAppBusiness.Infrastructure.Configuration.ConfigurationManager(configPath, sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<WhatsAppBusiness.Infrastructure.Configuration.ConfigurationManager>>()));
        services.AddSingleton<SecureConfigurationService>();

        // Register WhatsApp services
        services.AddSingleton<WhatsAppWebhookValidator>();
        services.AddSingleton<WhatsAppMessageMapper>();

        // Add WhatsApp HttpClient with resiliency
        services.AddHttpClient<WhatsAppClient>((sp, client) =>
        {
            var whatsAppSettings = configuration.GetSection("WhatsApp");
            var baseUrl = whatsAppSettings["ApiBaseUrl"] ?? "https://graph.facebook.com";
            var accessToken = whatsAppSettings["AccessToken"] ?? string.Empty;

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Register MCP services
        services.AddSingleton<MCPMessageFormatter>();

        // Add MCP HttpClient with resiliency
        services.AddHttpClient<MCPClient>((sp, client) =>
        {
            var mcpSettings = configuration.GetSection("MCP");
            var serverUrl = mcpSettings["ServerUrl"] ?? string.Empty;
            var apiKey = mcpSettings["ApiKey"] ?? string.Empty;

            if (!string.IsNullOrEmpty(serverUrl))
            {
                client.BaseAddress = new Uri(serverUrl);
            }

            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            client.Timeout = TimeSpan.FromSeconds(60);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    /// <summary>
    /// Gets retry policy for HTTP clients
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log retry attempt if needed
                });
    }

    /// <summary>
    /// Gets circuit breaker policy for HTTP clients
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }
}
