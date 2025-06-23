using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class LeadFieldDto
    {
        [JsonProperty("label")]
        public string? Label { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("updateable")]
        public bool Updateable { get; set; }

        [JsonProperty("sortable")]
        public bool Sortable { get; set; }

        [JsonProperty("createable")]
        public bool Createable { get; set; }
    }
}
