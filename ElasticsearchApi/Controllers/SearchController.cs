using Elastic.Clients.Elasticsearch;
using ElasticsearchApi.Models;
using Microsoft.AspNetCore.Mvc;
using ElasticsearchApi.Repository;

namespace ElasticsearchApi.Controllers
{
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
            _logger.LogInformation("Enter to SearchMessageByTextAsync");
            var result = await _client.GetReportsByTextAsync(text);
            if (result is null)
            {
                return StatusCode(500, "Failed to search Elasticsearch");
            }
            return Ok(result);
        }
    }
}
