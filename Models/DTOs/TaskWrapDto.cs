using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class TaskWrapDto
    {
        [JsonProperty("records")]
        public List<TaskDto>? Records { get; set; }
    }
}
