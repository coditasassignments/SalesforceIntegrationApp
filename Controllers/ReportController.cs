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
using SalesforceIntegrationApp.Services.Interfaces;
using SalesforceIntegrationApp.Filters;
namespace SalesforceIntegrationApp.Controllers
{
    [AuthorizeSession]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IReportService _reportService;
        public ReportController(ApplicationDbContext context, IReportService reportService)
        {
            _db = context;
            _reportService = reportService;
        }
        public async Task<ActionResult> FetchAndShowReports()
        {
            Logger.LogInfo("/Report/FetchAndShowReports called");
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                Logger.LogInfo("Session is null.Redirecting to Login.");
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Logger.LogInfo("Fetching report data from Salesforce");
                var reportData = await _reportService.FetchAndParseReportAsync();
                Logger.LogInfo("Saving fetched report data to database.");
                _reportService.SaveReportToDatabase(reportData);
                var savedData = _db.ReportDatas.ToList();
                Logger.LogInfo("Total no. of reports fetched and saved");
                var (paginatedData, totalPages, currentPage) = PaginationHelper.ApplyPagination(savedData, Request, 20);
                ViewBag.Columns = reportData.Columns;
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = currentPage;
                Logger.LogInfo("Displaying report data.");
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
    }
}
