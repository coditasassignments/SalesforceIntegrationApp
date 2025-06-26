using Azure.Core;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Models.DTOs;
using SalesforceIntegrationApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using SalesforceIntegrationApp.Logging;
using System.Threading.Tasks;
namespace SalesforceIntegrationApp.Services.Implementations
{
    public class InProgressService : IInProgressService //inheriting the IInprogressService interface
    {
        private readonly AuthService _authService;
        public InProgressService(AuthService authService)
        {
            _authService = authService;
        }
        public async Task<List<LeadOpenActivityDto>> GetLeadInProgressAsync()
        {
            Logger.LogInfo("Starting GetLeadInProgressAsync");
            try
            {
                var auth = await _authService.GetValidTokenAsync();
                using var client = new HttpClient(); //Creating an object client for HttpClient() class that is used to GET, POST Http requests.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string query = "SELECT Id, FirstName, LastName, Email, " +
                   "(SELECT Id, Subject, Status, ActivityDate FROM Tasks WHERE Status != 'Completed') " +
                   "FROM Lead";
                string url = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parsed = JsonConvert.DeserializeObject<LeadOpenActivityWrapDto>(json); //Deserializing the json data to c# objects.
                    Logger.LogInfo("Successfully fetched leads with open tasks.");
                    return parsed?.Records ?? new List<LeadOpenActivityDto>();
                }
                return new List<LeadOpenActivityDto>();
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred in GetLeadInProgressAsync", ex);
                return new List<LeadOpenActivityDto>();
            }
        }
        public async Task<List<ContactInProgressDto>> GetContactInProgressAsync()
        {
            Logger.LogInfo("Starting GetContactInProgressAsync");
            try
            {
                var auth = await _authService.GetValidTokenAsync();
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string query = "SELECT Id, FirstName, LastName, Email, " +
                   "(SELECT Id, Subject, Status, ActivityDate FROM Tasks WHERE Status != 'Completed') " +
                   "FROM Contact";
                string url = $"{auth.InstanceUrl!}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parsed = JsonConvert.DeserializeObject<ContactInProgressWrapDto>(json);
                    Logger.LogInfo("Successfully fetched contacts with open tasks.");
                    return parsed?.Records ?? new List<ContactInProgressDto>();
                }
                return new List<ContactInProgressDto>();
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred in GetContactInProgressAsync", ex);
                return new List<ContactInProgressDto>();
            }
        }
    }
}
