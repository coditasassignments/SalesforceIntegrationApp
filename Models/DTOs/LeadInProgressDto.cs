using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class LeadInProgressDto
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("WhoId")]
        public string WhoId { get; set; }

        [JsonProperty("Who")]
        public WhoDto Who { get; set; }
    }
}
