using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class ContactInProgressWrapDto
    {
        [JsonProperty("records")]
        public required List<ContactInProgressDto>? Records { get; set; }
    }
}

