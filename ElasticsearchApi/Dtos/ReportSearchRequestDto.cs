namespace ElasticsearchApi.Dtos;

public class ReportSearchRequestDto
{
    public string? Text { get; set; }
    public string? Theater { get; set; }
    public string? Sector { get; set; }
    public string? Location { get; set; }
    public string[]? Priorities { get; set; }
    public string? ReportType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
