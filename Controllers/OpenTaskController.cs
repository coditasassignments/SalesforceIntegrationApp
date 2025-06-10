using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Services.Interfaces;
using System.Net.Http.Headers;

public class OpenTaskController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IOpenTaskService _openTaskService;
    public OpenTaskController(ApplicationDbContext context, IOpenTaskService openTaskService)
    {
        _context = context;
        _openTaskService = openTaskService;
    }
    public async Task<IActionResult> FetchLeadsWithOpenTask()
    {
        var data = await _openTaskService.GetOpenTasksByTypeAsync("Lead");
        _context.LeadAndContactWithOpenTasks.AddRange(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("LeadWithOpenTask");
    }
    public async Task<IActionResult> FetchContactsWithOpenTask()
    {
        var data = await _openTaskService.GetOpenTasksByTypeAsync("Contact");
        _context.LeadAndContactWithOpenTasks.AddRange(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("ContactWithOpenTask");
    }
    public IActionResult LeadWithOpenTask()
    {
        var data = _context.LeadAndContactWithOpenTasks.ToList();
        return View("LeadWithOpenTask", data);
    }
    public IActionResult ContactWithOpenTask()
    {
        var data = _context.LeadAndContactWithOpenTasks.ToList();
        return View("ContactWithOpenTask", data);
    }
}
