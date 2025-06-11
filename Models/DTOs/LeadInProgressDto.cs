using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class LeadInProgressDto
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Company")]
        public string Company { get; set; }
    }
}
