using Consumer.Service;
using Consumer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<ConsumerService>();
        services.AddTransient<ContentValidateService>();
    })
    .Build();

var consumerService = host.Services.GetRequiredService<ConsumerService>();
consumerService.ConsumingData();