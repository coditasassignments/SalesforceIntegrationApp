using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class WhoDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }
    }
}
