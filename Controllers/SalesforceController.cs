using Microsoft.AspNetCore.Mvc;
using SalesforceIntegrationApp.Services.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Exceptions;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Services.Implementations;
using SalesforceIntegrationApp.Models.DTOs;
using SalesforceIntegrationApp.Filters;

namespace SalesforceIntegrationApp.Controllers
{

    [AuthorizeSession]
    [ResponseCache(Duration=0, Location=ResponseCacheLocation.None,NoStore=true)]
    public class SalesforceController : Controller
    {
        private readonly ISalesforceService _salesforceService;
        private readonly AuthService _authService;
        public SalesforceController(ISalesforceService salesforceService, AuthService authService)
        {
            _salesforceService = salesforceService;
            _authService = authService;
        }
        [HttpGet]
        public async Task<IActionResult> GetLeadMetaData()
        {
            Logger.LogInfo("/Salesforce/GetLeadMetaData called");
            try
            {
                Logger.LogInfo("Attempting to get a valid Salesforce token");
                var auth = await _authService.GetValidTokenAsync();
                Logger.LogInfo("Fetching Lead field metadata from Salesforce.");
                var selectedFields = await _salesforceService.GetLeadFieldsAsync(auth.AccessToken, auth.InstanceUrl);
                Logger.LogInfo("Total no. of fields retrieved");
                var (paginatedFields, totalPages, currentPage) = PaginationHelper.ApplyPagination(selectedFields, Request);
                ViewBag.Fields = paginatedFields;
                ViewBag.CurrentPage = currentPage;
                ViewBag.TotalPages = totalPages;

                Logger.LogInfo("Successfully fetched and paginated Lead metadata fields");
                return View("GetLeadMetaData");
            }
            catch (InvalidLeadDataException ex)
            {
                Logger.LogError("Invalid lead data error occurred.", ex);
                ViewBag.Error = ex.Message;
            }
            catch (Exception ex)
            {
                Logger.LogError("Unexpected error in GetLeadMetaData.", ex);
                ViewBag.Error = "Unexpected error occurred while fetching lead metadata.";
            }
            //ViewBag.Fields = new List<LeadFieldDto>(); ;
            ViewBag.Fields = new List<dynamic>();
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            return View("GetLeadMetaData");
        }
    }
}


        
