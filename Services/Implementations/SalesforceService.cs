using SalesforceIntegrationApp.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;  //Task<T> and Task classes come from
using Newtonsoft.Json;
using SalesforceIntegrationApp.Exceptions;
using SalesforceIntegrationApp.Logging;
using System;

namespace SalesforceIntegrationApp.Services.Implementations
{
    public class SalesforceService : ISalesforceService
    {
        public async Task<List<dynamic>> GetLeadFieldsAsync(string accessToken, string instanceUrl)
        {
            Logger.LogInfo("Started fetching lead metadata from Salesforce (Service Layer).");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); //Adding authorization header 
                var url = $"{instanceUrl}/services/data/v54.0/sobjects/Lead/describe";
                var response = await client.GetAsync(url);
                Logger.LogInfo($"Lead metadata fetch response: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) //if-condition in case of failing to get response throws exception
                    throw new Exception($"Error fetching lead metadata: {response.StatusCode} - {response.ReasonPhrase}");
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic parsed = JsonConvert.DeserializeObject(jsonResponse);
                //Implement jsonconvert deserialize using class

                var fields = parsed?.fields;
                if (fields == null)
                    throw new InvalidLeadDataException("Lead metadata does not contain any fields.");
                var selectedFields = new List<dynamic>();
                
                foreach (var field in fields)
                {
                    selectedFields.Add(new
                    {
                        label = field?.label ?? "N/A", // Usage of null-conditional and null-coaleascing operator
                        name = field?.name ?? "N/A",
                        updateable = field?.updateable ?? false,
                        sortable = field?.sortable ?? false,
                        createable = field?.createable ?? false
                    });
                }
                return selectedFields;
            }
        }
    }
}
