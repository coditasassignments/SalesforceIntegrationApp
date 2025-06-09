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
    public class DataService : IDataService
    {
        private readonly string accessToken = "00D90000000uBr5!AQsAQLJ2CHuKNC7NBWqVQRLboIe1Et3qE19cSBggBTjBbXJkcgwkSJ7O64GBznypK6JIPN2pQoKiRgS1_4p3NJnK1Jp0tuJe";
        private readonly string instanceUrl = "https://coditasdomain-dev-ed.my.salesforce.com";

        public async Task<List<Lead>> GetLeadsAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string query = "SELECT Id, FirstName, LastName, Company FROM Lead";
            string url = $"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic parsed = JsonConvert.DeserializeObject(json);
                return parsed.records.ToObject<List<Lead>>();
            }
            return new List<Lead>();
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string query = "SELECT Id, FirstName, LastName, Email FROM Contact";
            string url = $"{instanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic parsed = JsonConvert.DeserializeObject(json);
                return parsed.records.ToObject<List<Contact>>();
            }
            return new List<Contact>();
        }
    }
}
