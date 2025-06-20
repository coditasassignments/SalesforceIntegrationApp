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
namespace SalesforceIntegrationApp.Controllers
{
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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var reportData = await _reportService.FetchAndParseReportAsync();
                _reportService.SaveReportToDatabase(reportData);
                var savedData = _db.ReportDatas.ToList();
                var (paginatedData, totalPages, currentPage) = PaginationHelper.ApplyPagination(savedData, Request, 20);
                ViewBag.TotalPages = totalPages;
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
    }
}
