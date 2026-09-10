using ElasticsearchApi.Repository;
using Elastic.Clients.Elasticsearch;
using ElasticsearchApi.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/apifi-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true
    )
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddExceptionHandler<GlobalErrorHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSingleton(sp =>
    {
        string esUrl = builder.Configuration["ELASTICSEARCH_URL"] ?? "http://elasticsearch:9200";
        var settings = new ElasticsearchClientSettings(new Uri(esUrl))
            .DefaultIndex("reports-logs");

        return new ElasticsearchClient(settings);
    });
    builder.Services.AddScoped<IElasticSearchRepository, ElasticSearchRepository>();
    var app = builder.Build();
    app.UseExceptionHandler();

   
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
        Log.Fatal(ex, "ElasticsearchApi terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}