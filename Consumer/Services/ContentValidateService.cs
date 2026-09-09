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
        if(ReportModel is null)
        {
            _logger.LogError("connot desrialize the json raw !");
            return false;
        }
        bool subjectsAppear = CheckForSubjectAppear(Report);
        if (!subjectsAppear) { return false; }
    }
    public bool CheckForSubjectAppear(ReportModel Report)
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
}