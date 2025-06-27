using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Services.Interfaces;
using SalesforceIntegrationApp.Services.Implementations;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Filters;
[AuthorizeSession]
[ResponseCache(Duration=0, Location=ResponseCacheLocation.None,NoStore = true)]
public class InProgressController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IInProgressService _inProgressService;
    public InProgressController(ApplicationDbContext context, IInProgressService inProgressService)
    {
        _context = context;
        _inProgressService = inProgressService;
    }
    public async Task<IActionResult> GetLeadInProgress()
    {
        Logger.LogInfo("/InProgress/GetLeadInProgress called");
        try
        {
            Logger.LogInfo("Fetching in-progress leads from service.");
            var leadDtos = await _inProgressService.GetLeadInProgressAsync();
            var leads = leadDtos.Select(dto => new LeadOpenActivity // Mapping Dto to model
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            }).ToList();
            Logger.LogInfo("Total no. of leads in progress fetched");
            foreach (var lead in leads)
            {
                if (!_context.LeadsOpenActivity.Any(x => x.Id == lead.Id))
                    _context.LeadsOpenActivity.Add(lead);
            }
            await _context.SaveChangesAsync();
            Logger.LogInfo("Leads in progress saved to database.");
            var (paginatedLeads, totalPages, currentPage) = PaginationHelper.ApplyPagination(leads, Request);
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            return View("~/Views/Salesforce/LeadOpenActivity.cshtml", paginatedLeads);
        }
        catch(Exception ex)
        {
            Logger.LogError("Error occurred in GetLeadInProgress.", ex);
            ViewBag.Error = "An error occurred while fetching Lead In Progress data.";
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            return View("~/Views/Salesforce/LeadOpenActivity.cshtml", new List<LeadOpenActivity>());
        }

    }
    public async Task<IActionResult> GetContactInProgress()
    {
        Logger.LogInfo("/InProgress/GetContactInProgress called.");
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
        {
            Logger.LogInfo("User session is null.Redirecting to /Account/Login.");
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Logger.LogInfo("Fetching in-progress contacts from service");
            var contactDtos = await _inProgressService.GetContactInProgressAsync();
            var contacts = contactDtos.Select(dto => new ContactInProgress
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            }).ToList();
            Logger.LogInfo("Total no. of contacts in progress fetched");
            foreach (var contact in contacts)
            {
                if (!_context.ContactsInProgress.Any(x => x.Id == contact.Id))
                    _context.ContactsInProgress.Add(contact);
            }
            await _context.SaveChangesAsync();
            Logger.LogInfo("Contacts in progress saved to database");
            var (paginatedContacts, totalPages, currentPage) = PaginationHelper.ApplyPagination(contacts, Request);
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            return View("~/Views/Salesforce/ContactInProgress.cshtml", paginatedContacts);
        }
        catch(Exception ex)
        {
            Logger.LogError("Error occurred in GetContactInProgress.", ex);
            ViewBag.Error = "An error occurred while fetching Contact In Progress data.";
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            return View("~/Views/Salesforce/ContactInProgress.cshtml", new List<ContactInProgress>());
        }
    }
}
