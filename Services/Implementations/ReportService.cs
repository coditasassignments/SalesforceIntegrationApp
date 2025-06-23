using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Services.Interfaces;
using System.Net.Http.Headers;

namespace SalesforceIntegrationApp.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        private readonly AuthService _authService;
        private readonly string reportId = "00OGC00000N3zUc2AJ";
        public ReportService(ApplicationDbContext db, AuthService authService)
        {
            _db = db;
            _authService = authService;
        }
        public async Task<ReportDataModel> FetchAndParseReportAsync()
        {
            var auth = await _authService.GetValidTokenAsync();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            string url = $"{auth.InstanceUrl}/services/data/v54.0/analytics/reports/{reportId}";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new ReportDataModel(); 
            var json = await response.Content.ReadAsStringAsync();
            return ParseReportJson(json);
        }
        private ReportDataModel ParseReportJson(string json)
        {
            var jsonObj = JObject.Parse(json);
            var detailColumnKeys = jsonObj["reportMetadata"]?["detailColumns"] as JArray ?? new JArray();
            var columnMeta = jsonObj["reportExtendedMetadata"]?["detailColumnInfo"];
            var columns = new List<string>();
            foreach (var key in detailColumnKeys)
            {
                var label = columnMeta?[key?.ToString()]?["label"]?.ToString();
                columns.Add(label ?? key?.ToString() ?? "Column");
            }
            var rows = jsonObj["factMap"]?["T!T"]?["rows"] as JArray ?? new JArray();
            var dataRows = new List<List<string>>();
            foreach (var row in rows)
            {
                var rowData = row["dataCells"]?.Select(cell => cell["label"]?.ToString() ?? "").ToList() ?? new List<string>();
                dataRows.Add(rowData);
            }
            return new ReportDataModel
            {
                Columns = columns,
                Rows = dataRows
            };
        }
        public void SaveReportToDatabase(ReportDataModel reportData)
        {
            _db.ReportDatas.RemoveRange(_db.ReportDatas);
            _db.SaveChanges();
            foreach (var row in reportData.Rows!)
            {
                string jsonRow = JsonConvert.SerializeObject(row);
                _db.ReportDatas.Add(new ReportData { RowDataJson = jsonRow });
            }
            _db.SaveChanges(); 
        }
    }
}

