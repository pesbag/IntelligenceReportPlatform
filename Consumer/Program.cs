using Consumer.Service;
using Consumer.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

string esURL = Environment.GetEnvironmentVariable("ELASTICSEARCH_URL") ?? "http://elasticsearch:9200";
var settings = new ElasticsearchClientSettings(new Uri(esURL))
    .DefaultIndex("reports-logs");
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<ConsumerService>();
        services.AddTransient<ContentValidateService>();
        services.AddSingleton(new ElasticsearchClient(settings));
    })
    .Build();

var consumerService = host.Services.GetRequiredService<ConsumerService>();
await consumerService.ConsumingData();