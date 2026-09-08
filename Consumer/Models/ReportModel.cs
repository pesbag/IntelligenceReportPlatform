using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Consumer.Models;

public class ReportModel
{
    [JsonPropertyName("reportId")]
    public int ReportId { get; set; }
    public timestemp
}
