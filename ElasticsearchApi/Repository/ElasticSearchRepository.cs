using Elastic.Clients.Elasticsearch;
using ElasticsearchApi.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
namespace ElasticsearchApi.Repository;

public class ElasticSearchRepository: IElasticSearchRepository
{
    private readonly ILogger<ElasticSearchRepository> _logger;
    private readonly ElasticsearchClient _client;
    private const string IndexName = "reports-logs";

    public ElasticSearchRepository(ILogger<ElasticSearchRepository> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }
    public async Task<IEnumerable<ReportModel>?> GetReportsByTextAsync([FromQuery] string textToSearch)
    {
        _logger.LogInformation("enter to GetReportsByTextAsync function");
        var response = await _client.SearchAsync<ReportModel>(s => s
        .Indices(IndexName)
        .Query(q => q
            .Match(t => t
                .Field(x=>x.message)
                .Query(textToSearch)
            )
        )
    );
        if (response.IsValidResponse)
        {
            return response.Documents.ToList();
        }
        _logger.LogError("Error while asking resposne");
        return null;
    }
}
