using System;
namespace SalesforceIntegrationApp.Exceptions
{
    public class ReportFetchException : Exception
    {
        public ReportFetchException(string message) : base(message)
        {
        }
    }
}
