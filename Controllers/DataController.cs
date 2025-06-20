using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Services.Interfaces;
using SalesforceIntegrationApp.Services.Implementations;
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
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var leadDtos = await _dataService.GetLeadsAsync();
            var leads = leadDtos.Select(dto => new Lead // Mapping Dto to model
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Company = dto.Company,
                Email = dto.Email,
                Status = dto.Status,
                Title = dto.Title,
                Phone = dto.Phone
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
    [HttpPost]
    public async Task<IActionResult> UpdateLead([FromBody] Lead updatedLead)
    {
        try
        {
            var existingLead = _context.Leads.FirstOrDefault(l => l.Id == updatedLead.Id);
            if (existingLead == null)
                return NotFound();
            existingLead.FirstName = updatedLead.FirstName;
            existingLead.LastName = updatedLead.LastName;
            existingLead.Email = updatedLead.Email;
            existingLead.Phone = updatedLead.Phone;
            existingLead.Company = updatedLead.Company;
            existingLead.Status = updatedLead.Status;
            existingLead.Title = updatedLead.Title;
            await _context.SaveChangesAsync();
            var result = await _dataService.UpdateLeadInSalesforceAsync(updatedLead);
            if (result)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "Salesforce update failed" });
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in UpdateLead", ex);
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteLead(string id)
    {
        try
        {
            var leadInDb = _context.Leads.FirstOrDefault(l => l.Id == id);
            if (leadInDb != null)
            {
                _context.Leads.Remove(leadInDb);
                await _context.SaveChangesAsync();
            }
            var result = await _dataService.DeleteLeadFromSalesforceAsync(id);
            if (result)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "Salesforce delete failed" });
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in DeleteLead", ex);
            return Json(new { success = false, message = ex.Message });
        }
    }
    public async Task<IActionResult> GetContactData()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var contactDtos = await _dataService.GetContactsAsync();
            var contacts = contactDtos.Select(dto => new Contact
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Email = dto.Email,
                Title = dto.Title,

            }).ToList();
            foreach (var contact in contacts)
            {
                if (!_context.Contacts.Any(x => x.Id == contact.Id))
                    _context.Contacts.Add(contact);
            }
            await _context.SaveChangesAsync();
            Console.WriteLine("Db updated");
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
    [HttpPost]
    public async Task<IActionResult> UpdateContact([FromBody] Contact updatedContact)
    {
        try
        {
            var existingContact = _context.Contacts.FirstOrDefault(c => c.Id == updatedContact.Id);
            if (existingContact == null)
                return NotFound();
            existingContact.FirstName = updatedContact.FirstName;
            existingContact.LastName = updatedContact.LastName;
            existingContact.Phone = updatedContact.Phone;
            existingContact.Email = updatedContact.Email;
            existingContact.Title = updatedContact.Title;
            await _context.SaveChangesAsync();
            var result = await _dataService.UpdateContactInSalesforceAsync(updatedContact);
            if (result)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "Salesforce update failed" });
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in UpdateContact", ex);
            return Json(new { success = false, message = ex.Message });
        }
    }
}
