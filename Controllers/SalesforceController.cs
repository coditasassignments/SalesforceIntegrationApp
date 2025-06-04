using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Models;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Exceptions;
using SalesforceIntegrationApp.Logging;


namespace SalesforceIntegrationApp.Controllers
{

    public class SalesforceController : Controller
    {
        private readonly string instanceUrl = "https://coditasdomain-dev-ed.my.salesforce.com";
        private readonly string accessToken = "00D90000000uBr5!AQsAQPHyuAK5EaHnjVjs0n8fBtltEJVv0BybxorZClZjzxfugxteYYFWB8crmAPiZA5hpMc1ubLHxnwqO7NHssdC6b6PHDxM";

        private readonly ApplicationDbContext _context;
        public SalesforceController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> GetLeadMetaData()
        {
            try
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
                                name = field.name ?? "N/A",
                                updateable = field.updateable ?? false,
                                sortable = field.sortable ?? false,
                                createable = field.createable ?? false
                            });
                        }
                        int pageSize = 10;
                        int pageNumber = 1;

                        if (Request.Query.ContainsKey("page"))
                        {
                            int.TryParse(Request.Query["page"], out pageNumber);
                            if (pageNumber <= 0) pageNumber = 1;
                        }

                        int totalItems = selectedFields.Count;
                        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                        var paginatedFields = selectedFields
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        ViewBag.Fields = paginatedFields;
                        ViewBag.CurrentPage = pageNumber;
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
            }
            catch (InvalidLeadDataException ex)
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
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetLeadAndContactFields()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string query = "SELECT Id, FirstName, LastName, Email FROM Contact";
                string query2 = "SELECT Id, FirstName, LastName, Company FROM Lead";

                string urlContact = $"{instanceUrl}/services/data/v54.0/query/?q={Uri.EscapeDataString(query)}";
                string urlLead = $"{instanceUrl}/services/data/v54.0/query/?q={Uri.EscapeDataString(query2)}";

                var contactResponse = await client.GetAsync(urlContact);
                var contactData = new List<dynamic>();

                if (contactResponse.IsSuccessStatusCode)
                {
                    var json = await contactResponse.Content.ReadAsStringAsync();
                    dynamic parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    contactData = parsed.records.ToObject<List<dynamic>>();


                    foreach (var contact in contactData)
                    {
                        var contactModel = new Contact
                        {
                            SalesforceId = contact.Id ?? "N/A",
                            FirstName = contact.FirstName ?? "N/A",
                            LastName = contact.LastName ?? "N/A",
                            Email = contact.Email ?? "N/A"
                        };

                        _context.Contacts.Add(contactModel);
                    }

                    await _context.SaveChangesAsync();
                }

                var leadResponse = await client.GetAsync(urlLead);
                var leadData = new List<dynamic>();

                if (leadResponse.IsSuccessStatusCode)
                {
                    var json = await leadResponse.Content.ReadAsStringAsync();
                    dynamic parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    leadData = parsed.records.ToObject<List<dynamic>>();


                    foreach (var lead in leadData)
                    {
                        var leadModel = new Lead
                        {
                            SalesforceId = lead.Id ?? "N/A",
                            FirstName = lead.FirstName ?? "N/A",
                            LastName = lead.LastName ?? "N/A",
                            Company = lead.Company ?? "N/A"
                        };

                        _context.Leads.Add(leadModel);
                    }

                    await _context.SaveChangesAsync();
                }

                ViewBag.Contacts = contactData;
                ViewBag.Leads = leadData;

                return View("GetLeadAndContactFields");
            }
        }
        /*public async Task<IActionResult> ViewAllRecords()
        {
            var leads = await _context.Leads.ToListAsync();
            var contacts = await _context.Contacts.ToListAsync();

            ViewBag.Leads = leads;
            ViewBag.Contacts = contacts;

            return View("ViewAllRecords");
        }*/
        /*public async Task<IActionResult> FetchLeadAndContactWithOpenTask()
        {

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);


                    var leadQuery = "SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress' AND Who.Type = 'Lead'";
                    var contactQuery = "SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress' AND Who.Type = 'Contact'";

                    var leadResponse = await client.GetAsync($"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(leadQuery)}");
                    var contactResponse = await client.GetAsync($"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(contactQuery)}");

                    var allItems = new List<LeadAndContactWithOpenTask>();

                    if (leadResponse.IsSuccessStatusCode)
                    {
                        var json = await leadResponse.Content.ReadAsStringAsync();
                        dynamic parsed = JsonConvert.DeserializeObject(json);
                        foreach (var record in parsed.records)
                        {
                            allItems.Add(new LeadAndContactWithOpenTask
                            {
                                Label = "Lead",
                                Name = record?.Name ?? "N/A",
                                Email = record?.Email ?? "N/A",
                                Status = "Open"
                            });
                        }
                    }

                    if (contactResponse.IsSuccessStatusCode)
                    {
                        var json = await contactResponse.Content.ReadAsStringAsync();
                        dynamic parsed = JsonConvert.DeserializeObject(json);
                        foreach (var record in parsed.records ?? new List<dynamic>())
                        {
                            allItems.Add(new LeadAndContactWithOpenTask
                            {
                                Label = "Contact",
                                Name = record?.Name ?? "N/A",
                                Email = record?.Email ?? "N/A",
                                Status = "Open"
                            });
                        }
                    }


                    _context.LeadAndContactWithOpenTasks.RemoveRange(_context.LeadAndContactWithOpenTasks); // Clear old records
                    _context.LeadAndContactWithOpenTasks.AddRange(allItems);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("LeadAndContactWithOpenTask");
            }
            catch (Exception)
            {
                return Content("Unexpected error occurred while fetching and processing reports.");

            }
        } */
        public async Task<IActionResult> FetchLeadAndContactWithOpenTask()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var query = "SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress'";
                    var response = await client.GetAsync($"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}");

                    var allItems = new List<LeadAndContactWithOpenTask>();

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        dynamic parsed = JsonConvert.DeserializeObject(json);

                        foreach (var record in parsed.records)
                        {
                            string type = record?.Who?.Type;
                            string name = record?.Who?.Name ?? "N/A";
                            string email = record?.Who?.Email ?? "N/A";

                            if (type == "Lead" || type == "Contact")
                            {
                                allItems.Add(new LeadAndContactWithOpenTask
                                {
                                    Label = type,
                                    Name = name,
                                    Email = email,
                                    Status = "In Progress"
                                });
                            }
                        }
                        _context.LeadAndContactWithOpenTasks.RemoveRange(_context.LeadAndContactWithOpenTasks);
                        _context.LeadAndContactWithOpenTasks.AddRange(allItems);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("LeadAndContactWithOpenTask");
            }
            catch (Exception ex)
            {
                return Content("Unexpected error occurred while fetching and processing reports: " + ex.Message);
            }
        }


        public IActionResult LeadAndContactWithOpenTask()
        {
            var data = _context.LeadAndContactWithOpenTasks.ToList();
            return View("LeadAndContactWithOpenTask", data);
        }
    }

}

