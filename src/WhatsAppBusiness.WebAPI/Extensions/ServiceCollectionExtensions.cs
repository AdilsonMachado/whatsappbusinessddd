using WhatsAppBusiness.Infrastructure.Data;
using WhatsAppBusiness.WebAPI.Hubs;

namespace WhatsAppBusiness.WebAPI.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds WebAPI services to the service collection
    /// </summary>
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        // Add controllers with options
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
            });

        // Add API Explorer
        services.AddEndpointsApiExplorer();

        // Add SignalR
        services.AddSignalR();

        // Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            options.AddPolicy("Development", builder =>
            {
                builder.WithOrigins("http://localhost:3000", "http://localhost:5173")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        // Add Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<WhatsAppBusinessDbContext>();

        return services;
    }

    /// <summary>
    /// Adds Swagger/OpenAPI documentation
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        // OpenAPI disabled due to version conflicts with ASP.NET Core 9.0
        // Re-enable when versions are compatible
        // services.AddOpenApi();

        return services;
    }
}
