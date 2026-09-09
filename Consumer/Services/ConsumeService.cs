using Confluent.Kafka;
using Consumer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using System;
namespace Consumer.Service;
public class ConsumerService
{
    private readonly ILogger<ConsumerService> _logger;
    private readonly ContentValidateService _validator;
    public ConsumerService(ILogger<ConsumerService> logger, ContentValidateService validator) 
    {
        _logger = logger;
        _validator = validator;
    }
    public async Task ConsumingData()
    {
        string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS")!; // ?? "broker:9092"

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "report_10",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };
        //to end the program with ctrl+c
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("raw_data");
        Console.WriteLine("Subscribed to topic \"raw_data\"");
        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cts.Token);
                    string messagePayload = consumeResult.Message.Value;

                    Console.WriteLine($"received Partition: {consumeResult.Partition}, offset: {consumeResult.Offset}]");
                    Console.WriteLine($"data: {messagePayload}");
                    bool validRow = _validator.isValidRow(messagePayload);
                    if (!validRow) { continue; }
                    await _validator.SaveToESAsync(messagePayload);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Error: {ex.Message}");
                    Console.WriteLine($"error consuming message: {ex.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            Console.WriteLine("closing consumer application...");
        }
        finally
        {
            _logger.LogInformation("closing the consumer");
            consumer.Close();
        }
    }
    
}
