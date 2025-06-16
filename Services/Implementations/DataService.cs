using Azure.Core; // accessing Http response details
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; // for desrialization of json response into C# objects
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Models.DTOs;
using SalesforceIntegrationApp.Services.Interfaces;
using System; //core namespaces for c# for classes and base types
using System.Collections.Generic; //provides all the generic collection classes
using System.ComponentModel;
using System.Net.Http; //provide the classes for making Http requests and recieving the Http responses
using System.Net.Http.Headers; //namespace to manage the headers of the Http requests like (Authorization Bearer<access-token>, content-type application/json)
using System.Text;
using System.Threading.Tasks;  //provides async methods and Task<T>
namespace SalesforceIntegrationApp.Services.Implementations
{
    public class DataService : IDataService //inheriting the IDataService interface
    {

        private readonly AuthService _authService;
        public DataService(AuthService authService) // Injecting authService through Dependency Injection for accessing credentials
        {
            _authService = authService;
        }
        public async Task<List<LeadDto>> GetLeadsAsync() // method to make an api call for retrieving Leads data
        {
            var auth = await _authService.GetValidTokenAsync(); // storing credentials that are retrieved from authService
            using var client = new HttpClient(); //Creating an object client for HttpClient() class that is used to GET, POST Http requests.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken); // adding authorization header to the Http request that will be sent using client
            string query = "SELECT Id, FirstName, LastName, Company, Email, Status, Title, Phone FROM Lead";// query for retrieving Leads data
            string url = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}"; //url to make api get request by converting query to url encoded form
            var response = await client.GetAsync(url); //waiting to get the response of the GET request and storing inside response
            if (response.IsSuccessStatusCode) // if the response is successfull
            {
                var json = await response.Content.ReadAsStringAsync(); // Reading the json response to the string 
                var parsed = JsonConvert.DeserializeObject<LeadWrapDto>(json); //Deserializing the json string to c# objects.
                return parsed.Records; //Returning the records
            }
            return new List<LeadDto>(); // else return a new list
        }
        public async Task<bool> UpdateLeadInSalesforceAsync(Lead lead)
        {
            var auth = await _authService.GetValidTokenAsync();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            var updateFields = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(lead.FirstName)) updateFields["FirstName"] = lead.FirstName;
            if (!string.IsNullOrWhiteSpace(lead.LastName)) updateFields["LastName"] = lead.LastName;
            if (!string.IsNullOrWhiteSpace(lead.Company)) updateFields["Company"] = lead.Company;
            if (!string.IsNullOrWhiteSpace(lead.Email)) updateFields["Email"] = lead.Email;
            if (!string.IsNullOrWhiteSpace(lead.Status)) updateFields["Status"] = lead.Status;
            if (!string.IsNullOrWhiteSpace(lead.Title)) updateFields["Title"] = lead.Title;
            if (!string.IsNullOrWhiteSpace(lead.Phone)) updateFields["Phone"] = lead.Phone;
           var content = new StringContent(
                JsonConvert.SerializeObject(updateFields),
                Encoding.UTF8,
                "application/json"
            );

            var url = $"{auth.InstanceUrl}/services/data/v54.0/sobjects/Lead/{lead.Id}";
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };

            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteLeadFromSalesforceAsync(string id)
        {
            var token = await _authService.GetValidTokenAsync();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await client.DeleteAsync($"{token.InstanceUrl}/services/data/v54.0/sobjects/Lead/{id}");

            return response.IsSuccessStatusCode;
        }
        public async Task<List<ContactDto>> GetContactsAsync()
        {

            var auth = await _authService.GetValidTokenAsync();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            string query = "SELECT Id, FirstName, LastName, Phone, Email, Title FROM Contact";
            string url = $"{auth.InstanceUrl}/services/data/v54.0/query?q={Uri.EscapeDataString(query)}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("📦 Raw Salesforce JSON Response: " + json);
                var parsed = JsonConvert.DeserializeObject<ContactWrapDto>(json);
                return parsed.Records;
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine("❌ ERROR JSON: " + errorJson);
            }
            return new List<ContactDto>();
        }
        public async Task<bool> UpdateContactInSalesforceAsync(Contact contact)
        {
            var auth = await _authService.GetValidTokenAsync();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

            var updateFields = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(contact.FirstName)) updateFields["FirstName"] = contact.FirstName;
            if (!string.IsNullOrWhiteSpace(contact.LastName)) updateFields["LastName"] = contact.LastName;
            if (!string.IsNullOrWhiteSpace(contact.Phone)) updateFields["Phone"] = contact.Phone;
            if (!string.IsNullOrWhiteSpace(contact.Email)) updateFields["Email"] = contact.Email;
            if (!string.IsNullOrWhiteSpace(contact.Title)) updateFields["Title"] = contact.Title;

            var content = new StringContent(
                JsonConvert.SerializeObject(updateFields),
                Encoding.UTF8,
                "application/json"
            );

            var url = $"{auth.InstanceUrl}/services/data/v54.0/sobjects/Contact/{contact.Id}";
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };

            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

    }
}
