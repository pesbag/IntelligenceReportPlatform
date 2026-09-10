using ElasticsearchApi.Dtos;
using ElasticsearchApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElasticsearchApi.Repository;

public interface IElasticSearchRepository
{
    Task<IEnumerable<ReportModel>> GetReportsByTextAsync([FromQuery] string wordToSearch);
    Task<IEnumerable<ReportModel>> GetBysubjectSortedByTimeAsync(string subjectNumber);
    Task<IEnumerable<ReportModel>> GetReportsAsync(string? sector, string? location, string? theater, IEnumerable<string>? priorities, DateTime? from, DateTime? to);
    Task<AggregationSummaryDto> GetStatisticsAggregationAsync();
}
