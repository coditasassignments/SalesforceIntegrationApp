using SalesforceIntegrationApp.Models;

namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface IReportService
    {
        Task<ReportDataModel> FetchAndParseReportAsync();
        void SaveReportToDatabase(ReportDataModel reportData);
    }
}
