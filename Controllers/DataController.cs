using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Services.Interfaces;
public class DataController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDataService _dataService; 
    public DataController(ApplicationDbContext context, IDataService dataService) // Injecting the Database access layer and service layer through DI(Dependency Injection)
    {
        _context = context;
        _dataService = dataService;
    }
    public async Task<IActionResult> GetLeadData() // Controller method to fetch LeadsMetaData
    {
        try
        {
            var leadDtos = await _dataService.GetLeadsAsync();
            var leads = leadDtos.Select(dto => new Lead // Mapping Dto to model
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Company = dto.Company
            }).ToList();
            foreach (var lead in leads)
            {
                if (!_context.Leads.Any(x => x.Id == lead.Id))
                    _context.Leads.Add(lead);
            }
            await _context.SaveChangesAsync();
            var (paginatedLeads, totalPages, currentPage) = PaginationHelper.ApplyPagination(leads, Request);
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            return View("~/Views/Salesforce/Lead.cshtml", paginatedLeads);

        }
        catch(Exception ex)
        {
            Logger.LogError("Error occurred in GetLeadData.", ex);
            ViewBag.Error = "An error occurred while fetching lead data.";
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            return View("~/Views/Salesforce/Lead.cshtml", new List<Lead>());
        }
        
    }

    public async Task<IActionResult> GetContactData()
    {
        try
        {
            var contactDtos = await _dataService.GetContactsAsync();
            var contacts = contactDtos.Select(dto => new Contact
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            }).ToList();
            foreach (var contact in contacts)
            {
                if (!_context.Contacts.Any(x => x.Id == contact.Id))
                    _context.Contacts.Add(contact);
            }
            await _context.SaveChangesAsync();
            var (paginatedContacts, totalPages, currentPage) = PaginationHelper.ApplyPagination(contacts, Request);
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            return View("~/Views/Salesforce/Contact.cshtml", paginatedContacts);
        }
        catch(Exception ex)
        {
            Logger.LogError("Error occurred in GetContactData.", ex);
            ViewBag.Error = "An error occurred while fetching contact data.";
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            return View("~/Views/Salesforce/Contact.cshtml", new List<Contact>());
        }
    }

}
