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
        private readonly string accessToken = "00D90000000uBr5!AQsAQLJ2CHuKNC7NBWqVQRLboIe1Et3qE19cSBggBTjBbXJkcgwkSJ7O64GBznypK6JIPN2pQoKiRgS1_4p3NJnK1Jp0tuJe";
        private readonly string instanceUrl = "https://coditasdomain-dev-ed.my.salesforce.com";
        public async Task<List<LeadAndContactWithOpenTask>> GetOpenTasksByTypeAsync(string type)
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
                    queryUrl = null;
                }
            }
            return result;
        }
    }
}
