using Gait;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Gait.Services;
using Gait.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders(); // Remove all default providers including console
        // logging.AddFile("logs/app.log"); // if you have a file provider
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<OpenAIConfiguration>( context.Configuration.GetSection(OpenAIConfiguration.SectionName));

        services.AddSingleton<CommandRunner>();
        services.AddSingleton<GitService>();
        services.AddSingleton<AiService>();
        services.AddSingleton<ConsoleOutput>();

        services.AddTransient<App>();
    })
    .Build();

try
{
    var app = host.Services.GetRequiredService<App>();
    await app.RunAsync();
}
catch (Exception ex)
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while running the Gait application: {Message}", ex.Message);
    Environment.Exit(1);
}
