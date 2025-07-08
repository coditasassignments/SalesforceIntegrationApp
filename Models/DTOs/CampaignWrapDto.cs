using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class CampaignWrapDto
    {
        [JsonProperty("records")]
        public List<CampaignDto>? Records { get; set; }
    }
}
