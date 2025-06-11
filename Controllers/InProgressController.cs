using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Helpers;
using SalesforceIntegrationApp.Services.Interfaces;
using SalesforceIntegrationApp.Services.Implementations;
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
        var leadDtos = await _inProgressService.GetLeadInProgressAsync();
        var leads = leadDtos.Select(dto => new LeadInProgress // Mapping Dto to model
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Company = dto.Company
        }).ToList();
        foreach (var lead in leads)
        {
            if (!_context.LeadsInProgress.Any(x => x.Id == lead.Id))
                _context.LeadsInProgress.Add(lead);
        }
        await _context.SaveChangesAsync();
        var (paginatedLeads, totalPages, currentPage) = PaginationHelper.ApplyPagination(leads, Request);
        ViewBag.CurrentPage = currentPage;
        ViewBag.TotalPages = totalPages;
        return View("~/Views/Salesforce/LeadInProgress.cshtml", paginatedLeads);

    }
    public async Task<IActionResult> GetContactInProgress()
    {
        var contactDtos = await _inProgressService.GetContactInProgressAsync();
        var contacts = contactDtos.Select(dto => new ContactInProgress
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        }).ToList();
        foreach (var contact in contacts)
        {
            if (!_context.ContactsInProgress.Any(x => x.Id == contact.Id))
                _context.ContactsInProgress.Add(contact);
        }
        await _context.SaveChangesAsync();
        var (paginatedContacts, totalPages, currentPage) = PaginationHelper.ApplyPagination(contacts, Request);
        ViewBag.CurrentPage = currentPage;
        ViewBag.TotalPages = totalPages;
        return View("~/Views/Salesforce/ContactInProgress.cshtml", paginatedContacts);
    }
}
