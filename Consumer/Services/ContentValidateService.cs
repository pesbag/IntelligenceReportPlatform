using Consumer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Consumer.Models;
namespace Consumer.Services;


public class ContentValidateService
{
    private readonly ILogger<ContentValidateService> _logger;
    public ContentValidateService(ILogger<ContentValidateService> logger)
    {
        _logger = logger;
    }
    public bool isValidRow(string JsonMessage)
    {
        _logger.LogInformation("enter to validation function");
        var Report = JsonSerializer.Deserialize<ReportModel>(JsonMessage);
        if(Report is null)
        {
            _logger.LogError("connot desrialize the json raw !");
            return false;
        }
        if (!CheckForSubjectsAppear(Report) || !ValidateAllFields(Report))
        { return false; }
        return true;
    }
    public bool CheckForSubjectsAppear(ReportModel Report)
    {
        if ((!string.IsNullOrWhiteSpace(Report.subjectType) && (Report.subjectId is null))
            || (string.IsNullOrWhiteSpace(Report.subjectType) && (Report.subjectId is not null)))
        {
            _logger.LogInformation("Error: the json is not meet the system conditions" +
                "the reason: \'subjectType\' and \'subjectId\' both should appear or disappear");
            return false;
        }
        return true;
    }
    public bool ValidateAllFields(ReportModel Report)
    {
        if (!CheckForNullOrWhiteSpace(Report)) { return false; }
        if (!CheckForSubjectAppear(Report)) { return false; }
        if (!CheckForValidValuesInCategory(Report)) { return false; }
        if (!CheckForValidDateTime(Report)) { return false; }
        return true;
    }
    public bool CheckForValidDateTime(ReportModel Report)
    {
        if(Report.timestamp==DateTime.MinValue || Report.timestamp == default)
        {
            return false;
        }
        return true;
    }
    public bool CheckForValidValuesInCategory(ReportModel Report)
    {
        string[] validCategories = ["Observation", "Movement", "Meeting", "Access", "Communication", "Logistics", "Incident"];
        if (!validCategories.Contains(Report.priority.Trim()))
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
        return true;
    }
}