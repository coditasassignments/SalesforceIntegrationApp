using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesforceIntegrationApp.Models
{
    public class RepFolder
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? DeveloperName { get; set; }
        public string? Type { get; set; }
        public ICollection<Report>? Reports { get; set; }
    }
    public class Report
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? DeveloperName { get; set; }
        public string? Type { get; set; }
        public string? FolderDeveloperName { get; set; }
        [ForeignKey("FolderDeveloperName")]
        public RepFolder? Folder { get; set; }
    }
}
