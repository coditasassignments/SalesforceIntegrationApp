namespace SalesforceIntegrationApp.Models
{
    public class Lead
    {
        public int Id { get; set; } 
        public string? SalesforceId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
    }
}
