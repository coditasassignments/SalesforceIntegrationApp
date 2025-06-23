namespace SalesforceIntegrationApp.Models
{
    public class SalesforceAuth
    {
        public int? Id { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? RefreshToken { get; set; }
        public string? GrantType { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string? InstanceUrl { get; set; } = string.Empty;
        public DateTime? TokenLastUpdated { get; set; }
        public int? TokenValiditySeconds { get; set; } = 3600;

    }
}
