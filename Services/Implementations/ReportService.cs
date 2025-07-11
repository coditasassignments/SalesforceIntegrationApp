﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Logging;
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
            Logger.LogInfo("Starting FetchAndParseReportAsync");
            try
            {
                var auth = await _authService.GetValidTokenAsync();
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string url = $"{auth.InstanceUrl}/services/data/v54.0/analytics/reports/{reportId}";
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return new ReportDataModel();
                var json = await response.Content.ReadAsStringAsync();
                Logger.LogInfo("Report fetched successfully from Salesforce.");
                return ParseReportJson(json);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred in FetchAndParseReportAsync", ex);
                return new ReportDataModel();
            }
        }
        private ReportDataModel ParseReportJson(string json)
        {
            Logger.LogInfo("Parsing report JSON");
            try
            {
                var parse = JObject.Parse(json); //parsing the json response 
                var columnKeys = parse["reportMetadata"]?["detailColumns"] as JArray??new JArray(); // type-cast to json array the detailColumns array of the reportMetaDataSection
                var columnData = parse["reportExtendedMetadata"]?["detailColumnInfo"];
                var columns = new List<string>();
                foreach(var i in columnKeys)
                {
                    var tag = columnData?[i?.ToString()]?["label"]?.ToString();
                    columns.Add(tag??i?.ToString() ?? "Column");
                }
                var rows = parse["factMap"]?["T!T"]?["rows"] as JArray??new JArray();
                var dataRows = new List<List<string>>();
                foreach (var r in rows)
                {
                    var rowData = r["dataCells"]?.Select(cell => cell["label"]?.ToString() ?? "").ToList() ?? new List<string>();
                    dataRows.Add(rowData);
                }
                return new ReportDataModel
                {
                    Columns = columns,
                    Rows = dataRows
                };
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred while parsing report JSON", ex);
                return new ReportDataModel();
            }
        }
        public void SaveReportToDatabase(ReportDataModel reportData)
        {
            Logger.LogInfo("Saving report to database");
            try
            {
                _db.ReportDatas.RemoveRange(_db.ReportDatas);
                _db.SaveChanges();
                foreach (var row in reportData.Rows!)
                {
                    string jsonRow = JsonConvert.SerializeObject(row);
                    _db.ReportDatas.Add(new ReportData {RowDataJson = jsonRow});
                }
                _db.SaveChanges();
                Logger.LogInfo($"Saved rows to the database");
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred while saving report to the database", ex);
            }
        }
    }
}

