using BabyFoodChecklist.Application;
using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Infrastructure;
using BabyFoodChecklist.McpServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "McpServer_.log"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Baby Food Checklist MCP Server...");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog();

    // Reuse the same DI wiring as the API project
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // MCP server does not have HTTP context — tools accept userId as a parameter.
    // Register a no-op ICurrentUserService so the DI container can resolve Application handlers.
    builder.Services.AddScoped<ICurrentUserService, McpCurrentUserService>();

    // Register MCP server with stdio transport and all tool classes
    builder.Services.AddMcpServer()
        .WithStdioServerTransport()
        .WithTools<ProductTools>()
        .WithTools<EntryTools>()
        .WithTools<StatisticsTools>()
        .WithTools<NutritionAdvisorTools>();

    // MCP servers use stdio for communication, so suppress console logging
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    var app = builder.Build();

    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "MCP Server terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// No-op implementation of ICurrentUserService for the MCP server.
/// MCP tools accept userId as a parameter rather than from HttpContext.
/// </summary>
internal sealed class McpCurrentUserService : ICurrentUserService
{
    public Guid? UserId => null;
}
