using SalesforceIntegrationApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface IDataService
    {
        Task<List<Lead>> GetLeadsAsync();
        Task<List<Contact>> GetContactsAsync();
    }
}

