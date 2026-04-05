using BabyFoodChecklist.Application;
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
