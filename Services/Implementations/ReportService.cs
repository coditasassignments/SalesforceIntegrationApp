﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Exceptions;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Services.Interfaces;
using System.Net.Http.Headers;

namespace SalesforceIntegrationApp.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        private readonly string accessToken = "00D90000000uBr5!AQsAQOul9umrb3S8.vGzy2OoXbOARC2pO3pMX65x59sVhIjsym.9S1l9xlBOIEhH5W6k4LgqLDNf8fLVa4GhvA_1mwUiVWBd";
        private readonly string apiUrl = "https://coditasdomain-dev-ed.my.salesforce.com/services/data/v54.0/analytics/reports/00OGC00000N3zUc2AJ";
        public ReportService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<ReportDataModel> FetchAndParseReportAsync()
        {
            Logger.LogInfo("Fetching Salesforce report data...");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
                throw new ReportFetchException($"Failed to fetch report. Status: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            Logger.LogInfo("Parsing Salesforce report JSON...");
            return ParseReportJson(json);
        }
        private ReportDataModel ParseReportJson(string json)
        {
            var jsonObj = JObject.Parse(json);
            var rows = jsonObj["factMap"]["T!T"]["rows"] as JArray;
            var dataRows = new List<List<string>>();
            foreach (var row in rows)
            {
                var cells = row["dataCells"] as JArray;
                var rowData = new List<string>();
                foreach (var cell in cells)
                    rowData.Add(cell["label"]?.ToString() ?? "");
                dataRows.Add(rowData);
            }
            var columns = Enumerable.Range(1, dataRows[0].Count).Select(i => $"Column{i}").ToList();
            return new ReportDataModel
            {
                Columns = columns,
                Rows = dataRows
            };
        }
        public void SaveReportToDatabase(ReportDataModel reportData)
        {
            Logger.LogInfo("Saving report data to DB...");
            _db.ReportDatas.RemoveRange(_db.ReportDatas);
            _db.SaveChanges();
            foreach (var row in reportData.Rows)
            {
                string jsonRow = JsonConvert.SerializeObject(row);
                _db.ReportDatas.Add(new ReportData { RowDataJson = jsonRow });
            }
            _db.SaveChanges();
        }
    }
}
