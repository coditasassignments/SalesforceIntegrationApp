using System.Collections.Generic;
namespace SalesforceIntegrationApp.Models
{
    public class ReportDataModel
    {
        public List<string> Columns { get; set; }
        public List<List<string>> Rows { get; set; }
    }

    // Database entity jisme rows store karni hai
    public class ReportData
    {
        public int Id { get; set; }

        // Pure row ko JSON me store karenge kyunki columns dynamic hain
        public string RowDataJson { get; set; }
    }

}
