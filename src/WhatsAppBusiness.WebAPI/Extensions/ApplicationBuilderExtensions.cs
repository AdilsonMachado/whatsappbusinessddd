using WhatsAppBusiness.WebAPI.Hubs;
using WhatsAppBusiness.WebAPI.Middleware;

namespace WhatsAppBusiness.WebAPI.Extensions;

/// <summary>
/// Extension methods for IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the middleware pipeline
    /// </summary>
    public static IApplicationBuilder UseWebApiMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Request logging (first in pipeline)
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Exception handling
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // HTTPS redirection
        app.UseHttpsRedirection();

        // CORS
        if (env.IsDevelopment())
        {
            app.UseCors("Development");
        }
        else
        {
            app.UseCors("AllowAll");
        }

        // Routing
        app.UseRouting();

        // Authentication (placeholder for future JWT)
        app.UseAuthentication();

        // Authorization
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Maps endpoints
    /// </summary>
    public static IApplicationBuilder MapWebApiEndpoints(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseEndpoints(endpoints =>
        {
            // Map controllers
            endpoints.MapControllers();

            // Map SignalR hub
            endpoints.MapHub<ConversationHub>("/hubs/conversation");

            // Map health checks
            endpoints.MapHealthChecks("/health");

            // OpenAPI mapping disabled due to version conflicts
            // Re-enable when versions are compatible
            // if (env.IsDevelopment())
            // {
            //     endpoints.MapOpenApi();
            // }
        });

        return app;
    }
}
