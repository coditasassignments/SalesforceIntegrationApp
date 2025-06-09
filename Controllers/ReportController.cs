using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Data;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Models;
using Microsoft.EntityFrameworkCore;
using SalesforceIntegrationApp.Exceptions;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Helpers;
namespace SalesforceIntegrationApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReportController(ApplicationDbContext context)
        {
            _db = context;
        }
        public async Task<ActionResult> FetchAndShowReports()
        {
            try
            {
                Logger.LogInfo("Fetching Salesforce report data...");
                var reportJson = await FetchReportJsonFromSalesforce();

                if (reportJson == null)
                {
                    return Content("Failed to fetch report data");
                }
                var parsedReport = ParseReportJson(reportJson);
                Logger.LogInfo("Report parsed successfully.");
                SaveReportToDatabase(parsedReport);
                Logger.LogInfo("Report data saved to DB.");
                var savedData = _db.ReportDatas.ToList();
                var (paginatedData, totalPages, currentPage) = PaginationHelper.ApplyPagination(savedData, Request, pageSize: 10);ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = currentPage;
                return View("ReportView", paginatedData);
            }
            catch (ReportFetchException ex)
            {
                Logger.LogError("Error while fetching report from Salesforce.", ex);
                return Content($"Report Fetch Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError("Unexpected error in FetchAndShowReports", ex);
                return Content("Unexpected error occurred while fetching and processing reports.");
            }
        }
        private async Task<string> FetchReportJsonFromSalesforce()
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "00D90000000uBr5!AQsAQLJ2CHuKNC7NBWqVQRLboIe1Et3qE19cSBggBTjBbXJkcgwkSJ7O64GBznypK6JIPN2pQoKiRgS1_4p3NJnK1Jp0tuJe");
            string apiUrl = "https://coditasdomain-dev-ed.my.salesforce.com/services/data/v54.0/analytics/reports/00OGC00000N3zUc2AJ";
            var response = await httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new ReportFetchException($"Failed to fetch report from Salesforce. Status: {response.StatusCode}");
            }
            return await response.Content.ReadAsStringAsync();
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
                {
                    rowData.Add(cell["label"]?.ToString() ?? "");
                }
                dataRows.Add(rowData);
            }
            int colCount = dataRows[0].Count;
            var columns = new List<string>();
            for (int i = 0; i < colCount; i++)
            {
                columns.Add($"Column{i + 1}");
            }
            return new ReportDataModel
            {
                Columns = columns,
                Rows = dataRows
            };
        }
        private void SaveReportToDatabase(ReportDataModel reportData)
        {
            
            _db.ReportDatas.RemoveRange(_db.ReportDatas);
            _db.SaveChanges();
            foreach (var row in reportData.Rows)
            {
                var rowJson = JsonConvert.SerializeObject(row);
                _db.ReportDatas.Add(new ReportData
                {
                    RowDataJson = rowJson
                });
            }
            _db.SaveChanges();
        }
    }
}
