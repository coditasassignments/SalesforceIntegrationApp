namespace SalesforceIntegrationApp.Models.DTOs
{
    public class ReportFolderDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? DeveloperName { get; set; }
        public string? Type { get; set; }
    }
    public class ReportFolderWrapDto
    {
        public List<ReportFolderDto>? Records { get; set; }
    }
    public class ReportDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? DeveloperName { get; set; }
        public string? Type { get; set; }
        public string? FolderDeveloperName { get; set; }
    }
}
