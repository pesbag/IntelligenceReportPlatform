using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace Consumer.Models;

[Index(nameof(reportId),IsUnique=true)]
public class ReportModel
{
    [Required]
    public int reportId { get; set; }
    [Required]
    public DateTime timestamp { get; set; }
    [Required]
    public int agentId { get; set; }
    [Required]
    public string unit { get; set; } = string.Empty;
    [Required]
    public string theater { get; set; } = string.Empty;
    [Required]
    public string sector { get; set; } = string.Empty;
    [Required]
    public string location { get; set; } = string.Empty;
    [Required]
    [RegularExpression("^(Observation|Movement|Meeting|Access|Communication|Logistics|Incident)$")]
    public string reportType { get; set; } = string.Empty;
    [Required]
    [RegularExpression("^(Low|Medium|High|Critical)$")]
    public string priority { get; set; } = string.Empty;
    [Required]
    public string sourceType { get; set; } = string.Empty;
    [Required]
    public string message { get; set; } = string.Empty;
    [Required]
    public int? subjectId { get; set; }
    public string? subjectType { get; set; }
}
