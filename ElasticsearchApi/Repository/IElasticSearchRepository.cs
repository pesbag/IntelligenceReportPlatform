using ElasticsearchApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElasticsearchApi.Repository;

public interface IElasticSearchRepository
{
    Task<IEnumerable<ReportModel>> GetReportsByTextAsync([FromQuery] string wordToSearch);
    Task<IEnumerable<ReportModel>> GetBysubjectSortedByTimeAsync(string subjectNumber);
    Task<IEnumerable<ReportModel>> GetByCriteriaReportsAsync(string? Sector, string? Location, string? Theater);
}
