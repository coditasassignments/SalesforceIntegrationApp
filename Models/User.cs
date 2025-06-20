    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SalesforceIntegrationApp.Models
    {
        public class User
        {
            public int Id { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
            [NotMapped]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }
    }
