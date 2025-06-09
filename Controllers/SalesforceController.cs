using Microsoft.AspNetCore.Mvc;
using SalesforceIntegrationApp.Services.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Exceptions;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Helpers;


namespace SalesforceIntegrationApp.Controllers
{

    public class SalesforceController : Controller
    {
        private readonly ISalesforceService _salesforceService;
        private readonly string instanceUrl = "https://coditasdomain-dev-ed.my.salesforce.com";
        private readonly string accessToken = "00D90000000uBr5!AQsAQLJ2CHuKNC7NBWqVQRLboIe1Et3qE19cSBggBTjBbXJkcgwkSJ7O64GBznypK6JIPN2pQoKiRgS1_4p3NJnK1Jp0tuJe";
        public SalesforceController(ISalesforceService salesforceService)
        {
            _salesforceService = salesforceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetLeadMetaData()
        {
            /*try
            {
                Logger.LogInfo("Started fetching lead metadata from Salesforce.");
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var url = $"{instanceUrl}/services/data/v54.0/sobjects/Lead/describe";
                    var response = await client.GetAsync(url);
                    Logger.LogInfo($"Lead metadata fetch response: {response.StatusCode}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                        var fields = parsed?.fields;
                        if (fields == null)
                        {
                            throw new InvalidLeadDataException("Lead metadata does not contain any fields.");
                        }
                        var selectedFields = new List<dynamic>();
                        foreach (var field in fields)
                        {
                            selectedFields.Add(new
                            {
                                label = field?.label ?? "N/A",
                                name = field?.name ?? "N/A",
                                updateable = field?.updateable ?? false,
                                sortable = field?.sortable ?? false,
                                createable = field?.createable ?? false
                            });
                        }
                        var (paginatedFields, totalPages, currentPage) = PaginationHelper.ApplyPagination(selectedFields, Request); //Applying pagination logic
                        ViewBag.Fields = paginatedFields;
                        ViewBag.CurrentPage = currentPage;
                        ViewBag.TotalPages = totalPages;
                        return View("GetLeadMetaData");
                    }
                    else
                    {
                        ViewBag.Error = $"Error: {response?.StatusCode} - {response?.ReasonPhrase}";
                        ViewBag.Fields = new List<dynamic>();
                        ViewBag.CurrentPage = 1;
                        ViewBag.TotalPages = 1;
                        return View("GetLeadMetaData");
                    }
                }
            }*/
            try
            {
                var selectedFields = await _salesforceService.GetLeadFieldsAsync(accessToken, instanceUrl);
                var (paginatedFields, totalPages, currentPage) = PaginationHelper.ApplyPagination(selectedFields, Request);

                ViewBag.Fields = paginatedFields;
                ViewBag.CurrentPage = currentPage;
                ViewBag.TotalPages = totalPages;
                return View("GetLeadMetaData");
            }
            /*catch (InvalidLeadDataException ex)
            {
                Logger.LogError("Invalid lead data error occurred.", ex);
                ViewBag.Error = ex.Message;
                return View("GetLeadMetaData");
            }
            catch (Exception ex)
            {
                Logger.LogError("Unexpected error in GetLeadMetaData.", ex);
                ViewBag.Error = "Unexpected error occurred while fetching lead metadata.";
                return View("GetLeadMetaData");
            }*/
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
            ViewBag.Fields = new List<dynamic>();
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            return View("GetLeadMetaData");

        }
    }
}


        
