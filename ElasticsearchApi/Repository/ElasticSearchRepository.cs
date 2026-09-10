using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticsearchApi.Dtos;
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
    public async Task<IEnumerable<ReportModel>> GetReportsAsync(string? sector,string? location, string? theater, IEnumerable<string>? priorities, DateTime? from, DateTime? to)
    {
        _logger.LogInformation("Executing SearchReportsAsync function");

        var cleanPriorities = priorities?
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(FieldValue.String)
            .ToArray();

        var response = await _client.SearchAsync<ReportModel>(s => s
            .Indices(IndexName)
            .Query(q => q
                .Bool(b =>
                {
                    if (!string.IsNullOrWhiteSpace(sector))
                    {
                        b.Must(m => m.Match(t => t.Field(f => f.sector).Query(sector)));
                    }

                    if (!string.IsNullOrWhiteSpace(location))
                    {
                        b.Must(m => m.Match(t => t.Field(f => f.location).Query(location)));
                    }

                    if (!string.IsNullOrWhiteSpace(theater))
                    {
                        b.Must(m => m.Match(t => t.Field(f => f.theater).Query(theater)));
                    }

                    if (cleanPriorities != null && cleanPriorities.Length > 0)
                    {
                        b.Filter(f => f
                            .Terms(t => t
                                .Field(fld => fld.priority)
                                .Terms(new TermsQueryField(cleanPriorities))
                            )
                        );
                    }

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
                })
            )
        );

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException($"Elasticsearch combined search failed: {response.DebugInformation}");
        }

        return response.Documents.ToList();
    }

    public async Task<AggregationSummaryDto> GetStatisticsAggregationAsync()
    {
        _logger.LogInformation("Entering GetReportByPriorityAndTimeRangeAsync function");

        var response = await _client.SearchAsync<ReportModel>(s => s
            .Indices(IndexName)
            .Size(0)
            .Aggregations(a => a
                .Add("ReportsType", agg => agg.Terms(t => t.Field(f => f.reportType)))
                .Add("Priority", agg => agg.Terms(t => t.Field(f => f.priority)))
                .Add("Theater", agg => agg.Terms(t => t.Field(f => f.theater)))
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Error while asking resposne");
            throw new InvalidOperationException($"Aggregation failed: {response.DebugInformation}");
        }

        var summary = new AggregationSummaryDto();

        var typeBuckets = response.Aggregations.GetStringTerms("ReportsType")?.Buckets;
        if (typeBuckets != null)
        {
            summary.ReportsByType = typeBuckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount);
        }

        var priorityBuckets = response.Aggregations.GetStringTerms("Priority")?.Buckets;
        if (priorityBuckets != null)
        {
            summary.ReportsByPriority = priorityBuckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount);
        }

        var theaterBuckets = response.Aggregations.GetStringTerms("Theater")?.Buckets;
        if (theaterBuckets != null)
        {
            summary.ReportsByTheater = theaterBuckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount);
        }

        return summary;
    }
}
