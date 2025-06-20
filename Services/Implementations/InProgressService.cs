using Azure.Core;
using Newtonsoft.Json;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Models.DTOs;
using SalesforceIntegrationApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task<List<LeadInProgressDto>> GetLeadInProgressAsync()
        {
            var auth = await _authService.GetValidTokenAsync();
            using var client = new HttpClient(); //Creating an object client for HttpClient() class that is used to GET, POST Http requests.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            //string query = "SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress' AND Who.Type = 'Lead'";
            //string query = "SELECT Id, WhoId, Who.Name, Who.Email FROM Task WHERE Status IN ('In Progress', 'Not Started')";
            //AND WhoId LIKE '00Q%';
            string query = "SELECT Id, FirstName, LastName, Email, " +
               "(SELECT Id, Subject, Status, ActivityDate FROM Tasks WHERE Status != 'Completed') " +
               "FROM Lead";

            //SELECT Id, WhoId, Who.Name, Who.Email FROM Task WHERE Status IN ('In Progress', 'Not Started') AND WhoId LIKE '00Q%'
            //string query = "SELECT Id, Name, Email(SELECT Id, Status, Subject FROM Tasks WHERE Status IN ('In Progress', 'Not Started')) FROM Lead";
            //string query = SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress';
            //string query = "SELECT Lead.Id, Lead.FirstName, Lead.LastName(SELECT Id, Subject, Status FROM Tasks WHERE WhatId = Lead.Id AND STATUS IN('Open' , 'Not Started')) FROM Lead WHERE Lead.Id IN(SELECT WhatId FROM Task WHERE WhatId != NULL);
            string url = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonConvert.DeserializeObject<LeadInProgressWrapDto>(json); //Deserializing the json data to c# objects.
                return parsed.Records; //Converting the dynamic parsed to strongly typed list.
            }
            return new List<LeadInProgressDto>();
        }
        public async Task<List<ContactInProgressDto>> GetContactInProgressAsync()
        {
            var auth = await _authService.GetValidTokenAsync();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            string query = "SELECT Id, WhoId, Who.Name, Who.Email, Who.Type FROM Task WHERE Status = 'In Progress' AND Who.Type = 'Contact'";
            string url = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonConvert.DeserializeObject<ContactInProgressWrapDto>(json);
                return parsed.Records;
            }
            return new List<ContactInProgressDto>();
        }
    }
}
