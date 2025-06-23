using Newtonsoft.Json;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Models.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Implementations
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        public AuthService(ApplicationDbContext context) // injecting database access layer through dependency injection
        {
            _context = context;
        }
        public async Task<SalesforceAuth> GetValidTokenAsync() //Method to check the validity fof the token
        {
            var credentials = _context.SalesforceAuth.FirstOrDefault(); // Retriving credentials from the database
            if (credentials == null)  //In-case if record is null
                throw new Exception("Salesforce credentials not found in database."); //Throws exception
            if (IsTokenExpired(credentials)) //Checking if the access-token is expired 
            {
                credentials = await RefreshAccessTokenAsync(credentials); //in-case of expiry of access-token, calling method to make an api call for fetching new access-token from refresh token
            }
            return credentials; //else returning the valid credentials
        }
        private bool IsTokenExpired(SalesforceAuth credentials) // returns true if token expired else false
        {
            if (!credentials.TokenLastUpdated.HasValue)
                return true; // Returns true if tokens last updated value is null
            return (DateTime.UtcNow - credentials.TokenLastUpdated.Value).TotalSeconds >= credentials.TokenValiditySeconds; //checks the validity of the token
        }
        private async Task<SalesforceAuth> RefreshAccessTokenAsync(SalesforceAuth credentials) //method to make an api call for fething the access token with the help of refresh toke
        {
            using var httpClient = new HttpClient(); //creating an object for HttpClient class
            var form = new Dictionary<string, string> //storing credentials in a dictionary
            {
                { "grant_type", credentials.GrantType ?? throw new Exception("GrantType is missing") },
                { "client_id", credentials.ClientId ?? throw new Exception("ClientId is missing") },
                { "client_secret", credentials.ClientSecret ?? throw new Exception("ClientSecret is missing") },
                { "refresh_token", credentials.RefreshToken ?? throw new Exception("RefreshToken is missing") }
            };
            var response = await httpClient.PostAsync("https://coditasdomain-dev-ed.my.salesforce.com/services/oauth2/token", new FormUrlEncodedContent(form)); //making an api post call 
            if (!response.IsSuccessStatusCode) // if response is unsuccessfull
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to refresh Salesforce token. Status: {response.StatusCode}. Details: {errorDetails}"); // throws exception and return error details
            }
            var content = await response.Content.ReadAsStringAsync(); // asynchronous method to read response received in json format is converted to string
            var token = JsonConvert.DeserializeObject<SalesforceTokenResponseDto>(content); //Deserializes the response to convert it into an object
            if (string.IsNullOrEmpty(token.AccessToken) || string.IsNullOrEmpty(token.InstanceUrl))
            {
                throw new Exception("Token response missing required fields.");
            }
            credentials.AccessToken = token.AccessToken; //Mapping the DTO model to the actual model
            credentials.InstanceUrl = token.InstanceUrl;
            credentials.TokenLastUpdated = DateTime.UtcNow;
            _context.SalesforceAuth.Update(credentials); // Updating the value of credentials in the SalesforceAuth table
            await _context.SaveChangesAsync(); //saving all the changes in the database asynchronously
            return credentials; //returning the credentials
        }
    }
}
