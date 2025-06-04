using System;
namespace SalesforceIntegrationApp.Exceptions
{
    public class InvalidLeadDataException : Exception
    {
        public InvalidLeadDataException(string message) : base(message)
        {
        }
    }
}
