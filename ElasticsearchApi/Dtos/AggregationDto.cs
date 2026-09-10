namespace ElasticsearchApi.Dtos;

public class AggregationSummaryDto
{
    public Dictionary<string, long> ReportsByType { get; set; } = new();
    public Dictionary<string, long> ReportsByPriority { get; set; } = new();
    public Dictionary<string, long> ReportsByTheater { get; set; } = new();
}
