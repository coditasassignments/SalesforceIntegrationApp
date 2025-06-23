using System.Collections.Generic;
namespace SalesforceIntegrationApp.Models;
using System.ComponentModel.DataAnnotations.Schema;


public class ReportDataModel
{
    public List<string>? Columns { get; set; }
    public List<List<string>>? Rows { get; set; }
}
public class ReportData
{
    public int Id { get; set; } 
    public string? RowDataJson { get; set; }
}
