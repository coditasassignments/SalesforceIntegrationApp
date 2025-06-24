using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class SalesforceTokenResponseDto
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("instance_url")]
        public string? InstanceUrl { get; set; }

        [JsonProperty("id")]
        public string? UserIdUrl { get; set; } 

        [JsonProperty("token_type")]
        public string? TokenType { get; set; } 

        [JsonProperty("issued_at")]
        public string? IssuedAt { get; set; }

        [JsonProperty("signature")]
        public string? Signature { get; set; } 
    }
}
