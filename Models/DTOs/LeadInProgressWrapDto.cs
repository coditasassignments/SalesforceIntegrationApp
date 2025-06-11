using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class LeadInProgressWrapDto
    {
        [JsonProperty("records")]
        public List<LeadInProgressDto> Records { get; set; }
    }
}
