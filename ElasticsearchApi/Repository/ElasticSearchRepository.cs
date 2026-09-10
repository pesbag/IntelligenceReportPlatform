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
    public async Task<IEnumerable<ReportModel>> GetReportsByTextAsync(string textToSearch)
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
        if (!response.IsValidResponse)
        {
            _logger.LogError("Error while asking resposne");
            throw new InvalidOperationException($"Elasticsearch query failed: {response.DebugInformation}");
        }
        return response.Documents.ToList();
    }
    public async Task<IEnumerable<ReportModel>> GetBysubjectSortedByTimeAsync(string subjectNumber)
    {
        _logger.LogInformation("enter to GetBysubjectSortedByTimeAsync function");
        var response = await _client.SearchAsync<ReportModel>(s => s
        .Indices(IndexName)
        .Query(q => q
            .Match(t => t
                .Field(x => x.subjectId)
                .Query(subjectNumber)
                )
            )

            .Sort(s => s
          .Field(f => f.timestamp, new FieldSort { Order = SortOrder.Asc })
          )
        );
        if (!response.IsValidResponse)
        {
            _logger.LogError("Error while asking resposne");
            throw new InvalidOperationException($"Elasticsearch query failed: {response.DebugInformation}");
        }
        return response.Documents.ToList();
    }
    public async Task<IEnumerable<ReportModel>> GetByCriteriaReportsAsync(string? Sector,string? Location, string? Theater)
    {
        _logger.LogInformation("enter to GetByCriteriaReportsAsync function");
        var response = await _client.SearchAsync<ReportModel>(s => s
         .Indices(IndexName)
         .Query(q => q
             .Bool(b =>
             {
                 if (!string.IsNullOrWhiteSpace(Sector))
                 {
                     b.Must(m => m.Match(t => t.Field(f => f.sector).Query(Sector)));
                 }

                 if (!string.IsNullOrWhiteSpace(Location))
                 {
                     b.Must(m => m.Match(t => t.Field(f => f.location).Query(Location)));
                 }

                 if (!string.IsNullOrWhiteSpace(Theater))
                 {
                     b.Must(m => m.Match(t => t.Field(f => f.theater).Query(Theater)));
                 }
             })
         )
     );
        if (!response.IsValidResponse)
        {
            _logger.LogError("Error while asking resposne");
            throw new InvalidOperationException($"Elasticsearch criteria search failed: {response.DebugInformation}");
        }

        return response.Documents.ToList();
    }
    public async Task<IEnumerable<ReportModel>> GetReportByPriorityAndTimeRangeAsync(DateTime? from, DateTime? to, string? prior)
    {
        _logger.LogInformation("Entering GetReportByPriorityAndTimeRangeAsync with From: {From}, To: {To}, Priority: {Priority}",
            from, to, prior);

        var response = await _client.SearchAsync<ReportModel>(s => s
            .Indices(IndexName)
            .Query(q => q
                .Bool(b =>
                {
                    if (from.HasValue || to.HasValue)
                    {
                        b.Filter(f => f
                            .Range(r => r
                                .DateRange(d =>
                                {
                                    d.Field(m => m.timestamp);
                                    if (from.HasValue) d.Gte(from.Value);
                                    if (to.HasValue) d.Lte(to.Value);
                                })
                            )
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(prior))
                    {
                        b.Filter(f => f
                            .Match(m => m
                                .Field(x => x.priority)
                                .Query(prior)
                            )
                        );
                    }
                })
            )
        );

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException($"Elasticsearch criteria search failed: {response.DebugInformation}");
        }

        return response.Documents.ToList();
    }
}
