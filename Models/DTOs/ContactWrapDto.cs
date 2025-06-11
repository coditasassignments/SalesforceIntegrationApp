using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class ContactWrapDto
    {
        [JsonProperty("records")]
        public List<ContactDto> Records { get; set; }
    }
}

