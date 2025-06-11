using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class ContactDto
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }
    }
}
