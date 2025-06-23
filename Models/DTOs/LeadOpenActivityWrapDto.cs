using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class LeadOpenActivityWrapDto
    {
        [JsonProperty("records")]
        public required List<LeadOpenActivityDto>? Records { get; set; }
    }
}
