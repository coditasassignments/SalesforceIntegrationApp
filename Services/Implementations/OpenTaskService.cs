using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Implementations
{
    public class OpenTaskService : IOpenTaskService
    {
        private readonly AuthService _authService;
        public OpenTaskService(AuthService authService)
        {
            _authService = authService;
        }
        public async Task<List<LeadAndContactWithOpenTask>> GetOpenTasksByTypeAsync(string type)
        {
            var auth = await _authService.GetValidTokenAsync();
            var result = new List<LeadAndContactWithOpenTask>();
            string queryUrl = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString($"SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress' AND Who.Type = '{type}'")}";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
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
                    queryUrl = parsed.nextRecordsUrl != null? $"{auth.InstanceUrl}{parsed.nextRecordsUrl}": null;

                }
            }
            return result;
        }
    }
}
