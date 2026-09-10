using Elastic.Clients.Elasticsearch;
using ElasticsearchApi.Models;
using Microsoft.AspNetCore.Mvc;
using ElasticsearchApi.Repository;

namespace ElasticsearchApi.Controllers;

[ApiController]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly IElasticSearchRepository _client;
    private const string IndexName = "reports=logs";
    public SearchController(ILogger<SearchController> logger,
        IElasticSearchRepository client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpGet("api/reports/search")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> SearchMessageByTExtAsync([FromQuery] string text)
    {
        _logger.LogInformation("Enter to SearchMessageByTextAsync function in controller");
        var result = await _client.GetReportsByTextAsync(text);
        return Ok(result);
    }

    [HttpGet("api/subject{subjectId}/reports")]
    public async Task<ActionResult<IEnumerable<ReportModel>>?> SearchMessageBySubjectIdAscendingAsync(string subjectId)
    {
        _logger.LogInformation("enter to SearchMessageBySubjectIdAscendingAsync function in controller");
        var result = await _client.GetBysubjectSortedByTimeAsync(subjectId);
        return Ok(result);
    }

    [HttpGet("api/reports")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> SearchGivenParams(
    [FromQuery] string? sector,
    [FromQuery] string? location,
    [FromQuery] string? theater,
    [FromQuery] string[]? priorities,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
    {
        var results = await _client.GetReportsAsync(sector, location, theater, priorities, from, to);
        return Ok(results);
    }
   
    [HttpGet("api/reports/statistics")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetAllStatisticsAsync()
    {
        var result = await _client.GetStatisticsAggregationAsync();
        return Ok(result);
    }
}