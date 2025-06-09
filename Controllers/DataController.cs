using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Services.Interfaces;

public class DataController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDataService _dataService; 
    public DataController(ApplicationDbContext context, IDataService dataService)
    {
        _context = context;
        _dataService = dataService;
    }
    /*public async Task<IActionResult> GetLeadData()
    {
        var leadData = await FetchLeadDataFromSalesforce();
        foreach (var lead in leadData)
        {
            if (!_context.Leads.Any(x => x.Id == lead.Id))
                _context.Leads.Add(lead);
        }
        await _context.SaveChangesAsync();
        var (paginatedLeads, totalPages, currentPage) = PaginationHelper.ApplyPagination(leadData, Request); // Applying pagination helper
        ViewBag.CurrentPage = currentPage;
        ViewBag.TotalPages = totalPages;
        return View("~/Views/Salesforce/Lead.cshtml", paginatedLeads);
    }
    public async Task<IActionResult> GetContactData()
    {
        var contactData = await FetchContactDataFromSalesforce();
        foreach (var contact in contactData)
        {
            if (!_context.Contacts.Any(x => x.Id == contact.Id))
                _context.Contacts.Add(contact);
        }
        await _context.SaveChangesAsync();
        var (paginatedContacts, totalPages, currentPage) = PaginationHelper.ApplyPagination(contactData, Request); // Applying pagination Helper
        ViewBag.CurrentPage = currentPage;
        ViewBag.TotalPages = totalPages;
        return View("~/Views/Salesforce/Contact.cshtml", paginatedContacts);
    }
    private async Task<List<Lead>> FetchLeadDataFromSalesforce()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string query = "SELECT Id, FirstName, LastName, Company FROM Lead";
            string url = $"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic parsed = JsonConvert.DeserializeObject(json);
                var records = parsed.records.ToObject<List<Lead>>();
                return records;
            }
            return new List<Lead>();
        }
    }
    private async Task<List<Contact>> FetchContactDataFromSalesforce()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string query = "SELECT Id, FirstName, LastName, Email FROM Contact";
            string url = $"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic parsed = JsonConvert.DeserializeObject(json);
                var records = parsed.records.ToObject<List<Contact>>();
                return records;
            }
            return new List<Contact>();
        }
    }*/
    public async Task<IActionResult> GetLeadData()
    {
        var leadData = await _dataService.GetLeadsAsync();
        foreach (var lead in leadData)
        {
            if (!_context.Leads.Any(x => x.Id == lead.Id))
                _context.Leads.Add(lead);
        }
        await _context.SaveChangesAsync();

        var (paginatedLeads, totalPages, currentPage) = PaginationHelper.ApplyPagination(leadData, Request);
        ViewBag.CurrentPage = currentPage;
        ViewBag.TotalPages = totalPages;
        return View("~/Views/Salesforce/Lead.cshtml", paginatedLeads);
    }
    public async Task<IActionResult> GetContactData()
    {
        var contactData = await _dataService.GetContactsAsync();
        foreach (var contact in contactData)
        {
            if (!_context.Contacts.Any(x => x.Id == contact.Id))
                _context.Contacts.Add(contact);
        }
        await _context.SaveChangesAsync();

        var (paginatedContacts, totalPages, currentPage) = PaginationHelper.ApplyPagination(contactData, Request);
        ViewBag.CurrentPage = currentPage;
        ViewBag.TotalPages = totalPages;
        return View("~/Views/Salesforce/Contact.cshtml", paginatedContacts);
    }
}
