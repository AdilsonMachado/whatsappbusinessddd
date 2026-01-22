using Serilog;
using WhatsAppBusiness.Application;
using WhatsAppBusiness.Infrastructure;
using WhatsAppBusiness.WebAPI.Extensions;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/whatsapp-business-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting WhatsApp Business Web API");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                "logs/whatsapp-business-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
    });

    // Add services to the container
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddWebApiServices();
    builder.Services.AddSwaggerDocumentation();

    var app = builder.Build();

    // Configure middleware pipeline
    app.UseWebApiMiddleware(app.Environment);

    // Map endpoints
    app.MapWebApiEndpoints(app.Environment);

    Log.Information("WhatsApp Business Web API started successfully");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
