using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Consumer.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<ConsumerService>();
    })
    .Build();

var consumerService = host.Services.GetRequiredService<ConsumerService>();
consumerService.ConsumingData();