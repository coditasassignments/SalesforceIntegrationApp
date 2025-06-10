using SalesforceIntegrationApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface IInProgressService
    {
        Task<List<Lead>> GetLeadInProgressAsync();
        Task<List<Contact>> GetContactInProgressAsync();
    }
}
