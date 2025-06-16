using Newtonsoft.Json;

namespace SalesforceIntegrationApp.Models.DTOs
{
    public class ContactDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
    }
}
