using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class LeadWrapDto
    {
        [JsonProperty("records")]
        public List<LeadDto>? Records { get; set; }
    }
}
