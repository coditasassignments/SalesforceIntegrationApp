using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Models;

namespace SalesforceIntegrationApp.Controllers
{

    public class SalesforceController : Controller
    {
        private readonly string instanceUrl = "https://coditasdomain-dev-ed.my.salesforce.com";
        private readonly string accessToken = "00D90000000uBr5!AQsAQBB1wvXNmFV.VkqXg6DnDY3pqTyXfvONrJzvUxRZ6Fc7252cMK0jQ6yBHmKUmsLuIblnetGUK8vSjEmIitzh0aq7lEKt";

        private readonly ApplicationDbContext _context;
        public SalesforceController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> GetLeadMetaData()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var url = $"{instanceUrl}/services/data/v54.0/sobjects/Lead/describe";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    var fields = parsed.fields;
                    var selectedFields = new List<dynamic>();

                    foreach (var field in fields)
                    {
                        selectedFields.Add(new
                        {
                            label = field.label,
                            name = field.name,
                            updateable = field.updateable,
                            sortable = field.sortable,
                            createable = field.createable
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
                    ViewBag.Error = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    return View("GetLeadMetaData");
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLeadAndContactFields()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string query = "SELECT Id, FirstName, LastName, Email FROM Contact LIMIT 5";
                string query2 = "SELECT Id, FirstName, LastName, Company FROM Lead LIMIT 5";

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
                            SalesforceId = contact.Id,
                            FirstName = contact.FirstName,
                            LastName = contact.LastName,
                            Email = contact.Email
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
                            SalesforceId = lead.Id,
                            FirstName = lead.FirstName,
                            LastName = lead.LastName,
                            Company = lead.Company
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
        public async Task<IActionResult> ViewAllRecords()
        {
            var leads = await _context.Leads.ToListAsync();
            var contacts = await _context.Contacts.ToListAsync();

            ViewBag.Leads = leads;
            ViewBag.Contacts = contacts;

            return View("ViewAllRecords"); // 
        }





    }
}
