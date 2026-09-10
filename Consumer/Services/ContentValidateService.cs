using Consumer.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Consumer.Services;


public class ContentValidateService
{
    private readonly ILogger<ContentValidateService> _logger;
    private readonly ElasticsearchClient _esClient;
    public ContentValidateService(ILogger<ContentValidateService> logger, ElasticsearchClient esClient)
    {
        _logger = logger;
        _esClient = esClient;
    }
    public bool isValidRow(string JsonMessage)
    {
        _logger.LogInformation("enter to validation function for deserialize");
        //try
        //{
            var Report = JsonSerializer.Deserialize<ReportModel>(JsonMessage);
            if (Report is null)
            {
                _logger.LogError("connot desrialize the json raw !");
                return false;
            }
            if (!CheckForSubjectsAppear(Report) || !ValidateAllFields(Report))
            { return false; }
            return true;
        //}
        //catch (JsonException ex)
        //{
        //    _logger.LogError($"Error while parsing the data {ex.Message} continue to the next row");
        //    return false;
        //}
    }
    public bool CheckForSubjectsAppear(ReportModel report)
    {
        _logger.LogInformation("Entering CheckForSubjectsAppear");

        bool hasType = !string.IsNullOrWhiteSpace(report.subjectType);
        bool hasId = !string.IsNullOrWhiteSpace(report.subjectId);

        if (hasType ^ hasId)
        {
            _logger.LogWarning("Validation failed: 'subjectType' and 'subjectId' must either both appear or both be omitted");
            return false;
        }

        return true;
    }
    public bool ValidateAllFields(ReportModel Report)
    {
        _logger.LogInformation("enter to validate all fields function");
        if (!CheckForNullOrWhiteSpace(Report)) { return false; }
        var subjectAppear = CheckForSubjectsAppear(Report);
        if (subjectAppear) 
        {
            if (!CheckForValidValuesInCategory(Report)) { return false; }
            if (!CheckForValidValuesInPriority(Report)) { return false; }
        }
        //{ return false; }
        if (!CheckForValidDateTime(Report)) { return false; }
        return true;
    }
    public bool CheckForValidDateTime(ReportModel Report)
    {
        if(!DateTimeOffset.TryParse(Report.timestamp,out var paredDate))
        {
            _logger.LogError("Error: timestamp is not in leggal format");
            return false;
        }
        return true;
    }
    public bool CheckForValidValuesInCategory(ReportModel Report)
    {
        string[] validCategories = ["Observation", "Movement", "Meeting", "Access", "Communication", "Logistics", "Incident"];
        if (!validCategories.Contains(Report.reportType.Trim()))
        {
            _logger.LogError($"error in report number {Report.reportId} reportType is not leggal");
            return false;
        }
        return true;
    }
    public bool CheckForValidValuesInPriority(ReportModel Report)
    {
        string[] validPriority = ["Low", "Medium", "High", "Critical"];
        if (!validPriority.Contains(Report.priority.Trim())) 
        {
            _logger.LogError($"error in report number {Report.reportId} priority is not leggal");
            return false;
        }
        return true;
    }
    public bool CheckForNullOrWhiteSpace(ReportModel Report)
    {
        if(string.IsNullOrWhiteSpace(Report.unit))
        {
            _logger.LogError($"in report number: {Report.reportId} unit is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.timestamp))
        {
            _logger.LogError($"in report number: {Report.reportId} timestamp is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.theater))
        {
            _logger.LogError($"in report number: {Report.reportId} threater is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.sector))
        {
            _logger.LogError($"in report number: {Report.reportId} sector is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.reportType))
        {
            _logger.LogError($"in report number: {Report.reportId} reportType is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.priority))
        {
            _logger.LogError($"in report number: {Report.reportId} priority is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.sourceType))
        {
            _logger.LogError($"in report number: {Report.reportId} sourceType is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.message))
        {
            _logger.LogError($"in report number: {Report.reportId} message is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.location))
        {
            _logger.LogError($"in report number: {Report.reportId} location is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.agentId))
        {
            _logger.LogError($"in report number: {Report.reportId} agentId is missing");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Report.reportId))
        {
            _logger.LogError($"in report number: {Report.reportId} reportId is missing");
            return false;
        }
        
        return true;
    }
    public async Task SaveToESAsync(string message)
    {
        _logger.LogInformation("enter to SaveToESAsync func");
        var Report = JsonSerializer.Deserialize<ReportModel>(message);
        Report.createdAt = DateTime.UtcNow;
        await _esClient.IndexAsync(Report, idx => idx
        .Index("reports-logs")
        .Id(Report.reportId));
        _logger.LogInformation("save message to reports-logs index");
    }
}