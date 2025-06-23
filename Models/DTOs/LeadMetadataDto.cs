using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class LeadMetadataDto
    {
        [JsonProperty("fields")]
        public List<LeadFieldDto>? Fields { get; set; }
    }
}
