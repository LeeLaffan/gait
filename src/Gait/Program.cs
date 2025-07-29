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
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<OpenAIConfiguration>( context.Configuration.GetSection(OpenAIConfiguration.SectionName));

        services.AddSingleton<CommandRunner>();
        services.AddSingleton<GitDiffService>();
        services.AddSingleton<AiService>();

        services.AddTransient<App>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
            options.SingleLine = true;
        });
        logging.SetMinimumLevel(LogLevel.Information);
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
