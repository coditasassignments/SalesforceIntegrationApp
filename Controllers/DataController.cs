using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Services.Interfaces;
using SalesforceIntegrationApp.Services.Implementations;
using SalesforceIntegrationApp.Filters;
[AuthorizeSession] //custom attribute to check user is logged in or not
[ResponseCache(Duration=0,Location=ResponseCacheLocation.None,NoStore=true)] //to avoid caching of secured pages in the browser
public class DataController : Controller
{
    private readonly ApplicationDbContext _context; 
    private readonly IDataService _dataService; 
    public DataController(ApplicationDbContext context, IDataService dataService) //Injecting the Database access layer and service layer through DI(Dependency Injection)
    {
        _context = context;
        _dataService = dataService;
    }
    public async Task<IActionResult> GetLeadData() //Controller method to fetch Leads Data.
    {
        Logger.LogInfo("/Data/GetLeadData called"); //Added logger.info method of the logger class
        try
        {
            Logger.LogInfo("Fetching leads from Salesforce.");
            var leadDtos = await _dataService.GetLeadsAsync(); //fetches the lead data from the GetLeadsAsync method of dataService 
            var leads = leadDtos.Select(dto => new Lead //Mapping Dto to model
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
            Logger.LogInfo("Total no. of leads fetched"); //added a logger function to fetch total no. of leads that are fetched
            foreach (var lead in leads) //iterating through the leads data
            {
                if (!_context.Leads.Any(x => x.Id == lead.Id)) //if there are no leads in the database
                    _context.Leads.Add(lead); //add all the records to the 'Leads' table
            }
            await _context.SaveChangesAsync(); //otherwise save changes in the database
            Logger.LogInfo("Leads saved to database.");
            var (paginatedLeads, totalPages, currentPage) = PaginationHelper.ApplyPagination(leads, Request); //Now applying the pagination logic to the list of records fetched
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            return View("~/Views/Salesforce/Lead.cshtml", paginatedLeads); //returns the view to display the lead records

        }
        catch(Exception ex)
        {
            Logger.LogError("Error occurred in GetLeadData.", ex);
            ViewBag.Error = "An error occurred while fetching lead data.";
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            return View("~/Views/Salesforce/Lead.cshtml", new List<Lead>()); //if there is error in fetching leads,simply returns empty list
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateLead([FromBody] Lead updatedLead) 
    {
        Logger.LogInfo($"/Data/UpdateLead called for Lead ID: {updatedLead.Id}");
        try
        {
            var existingLead = _context.Leads.FirstOrDefault(l => l.Id == updatedLead.Id);
            if (existingLead == null)
            {
                Logger.LogInfo("Lead not found");
                return NotFound();
            }
            existingLead.FirstName = updatedLead.FirstName;
            existingLead.LastName = updatedLead.LastName;
            existingLead.Email = updatedLead.Email;
            existingLead.Phone = updatedLead.Phone;
            existingLead.Company = updatedLead.Company;
            existingLead.Status = updatedLead.Status;
            existingLead.Title = updatedLead.Title;
            await _context.SaveChangesAsync();
            Logger.LogInfo("Lead updated in local database");
            var result = await _dataService.UpdateLeadInSalesforceAsync(updatedLead);
            if (result)
            {
                Logger.LogInfo($"Lead updated in Salesforce");
                return Json(new{success = true});
            }
            else
            {
                Logger.LogInfo("Salesforce update failed");
                return Json(new{success = false, message = "Salesforce update failed"});
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in UpdateLead", ex);
            return Json(new{success = false, message = ex.Message});
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteLead(string id)
    {
        Logger.LogInfo("/Data/DeleteLead called");

        try
        {
            var result = await _dataService.DeleteLeadFromSalesforceAsync(id);
            if (result)
            {
                Logger.LogInfo("Lead deleted from Salesforce");
                var leadInDb = _context.Leads.FirstOrDefault(l => l.Id == id);
                if (leadInDb != null)
                {
                    _context.Leads.Remove(leadInDb);
                    await _context.SaveChangesAsync();
                    Logger.LogInfo("Lead deleted from database");
                }
                return Json(new { success = true });
            }
            else
            {
                Logger.LogInfo("Salesforce deletion failed");
                return Json(new { success = false, message = "Salesforce delete failed" });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in DeleteLead", ex);
            return Json(new { success = false, message = ex.Message });
        }
    }
    public async Task<IActionResult> GetContactData()
    {
        Logger.LogInfo("/Data/GetContactData called");
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
        {
            Logger.LogInfo("User session is null.Redirecting to Login.");
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Logger.LogInfo("Fetching contacts from Salesforce");
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
            Logger.LogInfo("Total no. of contacts fetched");
            foreach (var contact in contacts)
            {
                if (!_context.Contacts.Any(x => x.Id == contact.Id))
                    _context.Contacts.Add(contact);
            }
            await _context.SaveChangesAsync();
            Logger.LogInfo("Contacts saved to database.");
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
        Logger.LogInfo("/Data/UpdateContact called");
        try
        {
            var existingContact = _context.Contacts.FirstOrDefault(c => c.Id == updatedContact.Id);
            if (existingContact == null)
            {
                Logger.LogInfo($"Contact not found: {updatedContact.Id}");
                return NotFound();
            }
            existingContact.FirstName = updatedContact.FirstName;
            existingContact.LastName = updatedContact.LastName;
            existingContact.Phone = updatedContact.Phone;
            existingContact.Email = updatedContact.Email;
            existingContact.Title = updatedContact.Title;
            await _context.SaveChangesAsync();
            Logger.LogInfo($"Contact updated in local DB: {updatedContact.Id}");
            var result = await _dataService.UpdateContactInSalesforceAsync(updatedContact);
            if (result)
            {
                Logger.LogInfo($"Contact updated in Salesforce: {updatedContact.Id}");
                return Json(new{success = true});
            }
            else
            {
                Logger.LogInfo($"Salesforce update failed for Contact ID: {updatedContact.Id}");
                return Json(new{success = false, message = "Salesforce update failed" });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in UpdateContact", ex);
            return Json(new {success = false, message = ex.Message});
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteContact(string id)
    {
        Logger.LogInfo("/Data/DeleteContact called");

        try
        {
            var result = await _dataService.DeleteContactFromSalesforceAsync(id);
            if (result)
            {
                Logger.LogInfo("Contact deleted from Salesforce");
                var contactInDb = _context.Contacts.FirstOrDefault(l => l.Id == id);
                if (contactInDb != null)
                {
                    _context.Contacts.Remove(contactInDb);
                    await _context.SaveChangesAsync();
                    Logger.LogInfo("Contact deleted from database");
                }
                return Json(new { success = true });
            }
            else
            {
                Logger.LogInfo("Salesforce deletion failed");
                return Json(new { success = false, message = "Salesforce delete failed" });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in DeleteLead", ex);
            return Json(new { success = false, message = ex.Message });
        }
    }
}
