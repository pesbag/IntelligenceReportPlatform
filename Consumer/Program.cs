using Consumer.Service;
using Consumer.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/consumer-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true
    )
    .CreateLogger();

string esURL = Environment.GetEnvironmentVariable("ELASTICSEARCH_URL") ?? "http://elasticsearch:9200";
var settings = new ElasticsearchClientSettings(new Uri(esURL))
    .DefaultIndex("reports-logs");
try
{
    var host = Host.CreateDefaultBuilder(args).UseSerilog()
        .ConfigureServices((context, services) =>
        {
            services.AddTransient<ConsumerService>();
            services.AddTransient<ContentValidateService>();
            services.AddSingleton(new ElasticsearchClient(settings));
        })
        .Build();

    var consumerService = host.Services.GetRequiredService<ConsumerService>();
    await consumerService.ConsumingData();
}
catch(Exception ex)
{
    Log.Fatal(ex, "consumer terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}