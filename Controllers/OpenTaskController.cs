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
    /*public async Task<IActionResult> FetchLeadsWithOpenTask()
    {
        var data = await FetchOpenTasksByType("Lead");
        _context.LeadAndContactWithOpenTasks.RemoveRange(_context.LeadAndContactWithOpenTasks);
        _context.LeadAndContactWithOpenTasks.AddRange(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("LeadWithOpenTask");
    }*/
    public async Task<IActionResult> FetchLeadsWithOpenTask()
    {
        var data = await _openTaskService.GetOpenTasksByTypeAsync("Lead");
        _context.LeadAndContactWithOpenTasks.RemoveRange(_context.LeadAndContactWithOpenTasks);
        _context.LeadAndContactWithOpenTasks.AddRange(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("LeadWithOpenTask");
    }
    /*public async Task<IActionResult> FetchContactsWithOpenTask()
    {
        var data = await FetchOpenTasksByType("Contact");
        _context.LeadAndContactWithOpenTasks.RemoveRange(_context.LeadAndContactWithOpenTasks);
        _context.LeadAndContactWithOpenTasks.AddRange(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("ContactWithOpenTask");
    }*/
    public async Task<IActionResult> FetchContactsWithOpenTask()
    {
        var data = await _openTaskService.GetOpenTasksByTypeAsync("Contact");
        _context.LeadAndContactWithOpenTasks.RemoveRange(_context.LeadAndContactWithOpenTasks);
        _context.LeadAndContactWithOpenTasks.AddRange(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("ContactWithOpenTask");
    }
    /*public async Task<IActionResult> FetchContactsWithOpenTask()
    {
        var data = await _openTaskService.GetOpenTasksByTypeAsync("Contact");
        _context.LeadAndContactWithOpenTasks.RemoveRange(_context.LeadAndContactWithOpenTasks);
        _context.LeadAndContactWithOpenTasks.AddRange(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("ContactWithOpenTask");
    }

    private async Task<List<LeadAndContactWithOpenTask>> FetchOpenTasksByType(string type)
    {
        var result = new List<LeadAndContactWithOpenTask>();
        string queryUrl = $"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString($"SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress' AND Who.Type = '{type}'")}";
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            while (!string.IsNullOrEmpty(queryUrl))
            {
                var response = await client.GetAsync(queryUrl);
                if (!response.IsSuccessStatusCode)
                    break;
                var json = await response.Content.ReadAsStringAsync();
                dynamic parsed = JsonConvert.DeserializeObject(json);
                foreach (var record in parsed.records)
                {
                    string name = record?.Who?.Name ?? "N/A";
                    string email = record?.Who?.Email ?? "N/A";
                    result.Add(new LeadAndContactWithOpenTask
                    {
                        Label = type,
                        Name = name,
                        Email = email,
                        Status = "In Progress"
                    });
                }
            }
        }

        return result;
    }
    public IActionResult LeadWithOpenTask()
    {
        var data = _context.LeadAndContactWithOpenTasks.ToList();
        return View("LeadWithOpenTask", data);
    }*/
    public IActionResult LeadWithOpenTask()
    {
        var data = _context.LeadAndContactWithOpenTasks.ToList();
        return View("LeadWithOpenTask", data);
    }

    /*public IActionResult ContactWithOpenTask()
    {
        var data = _context.LeadAndContactWithOpenTasks.ToList();
        return View("ContactWithOpenTask", data);
    }*/
    public IActionResult ContactWithOpenTask()
    {
        var data = _context.LeadAndContactWithOpenTasks.ToList();
        return View("ContactWithOpenTask", data);
    }
}
