using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WhatsAppBusiness.Application.Interfaces;
using WhatsAppBusiness.Application.Services;

namespace WhatsAppBusiness.Application;

/// <summary>
/// Dependency injection configuration for the Application layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application layer services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Add AutoMapper
        services.AddAutoMapper(assembly);

        // Add FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Add Application Services
        services.AddScoped<IConversationService, ConversationApplicationService>();
        services.AddScoped<MessageApplicationService>();
        services.AddScoped<OperatorApplicationService>();

        return services;
    }
}
