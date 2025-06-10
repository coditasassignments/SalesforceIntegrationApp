using Azure.Core;
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
    public class DataService : IDataService //inheriting the IDataService interface
    {

        private readonly AuthService _authService;
        public DataService(AuthService authService)
        {
            _authService = authService;
        }
        public async Task<List<Lead>> GetLeadsAsync()
        {
            var auth = await _authService.GetValidTokenAsync();

            using var client = new HttpClient(); //Creating an object client for HttpClient() class that is used to GET, POST Http requests.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            string query = "SELECT Id, FirstName, LastName, Company FROM Lead";
            string url = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic parsed = JsonConvert.DeserializeObject(json); //Deserializing the json data to c# objects.
                return parsed.records.ToObject<List<Lead>>(); //Converting the dynamic parsed to strongly typed list.
            }
            return new List<Lead>();
        }
        public async Task<List<Contact>> GetContactsAsync()
        {
            var auth = await _authService.GetValidTokenAsync();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            string query = "SELECT Id, FirstName, LastName, Email FROM Contact";
            string url = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
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
