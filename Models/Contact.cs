using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models
{
    public class Contact
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
    }
}
