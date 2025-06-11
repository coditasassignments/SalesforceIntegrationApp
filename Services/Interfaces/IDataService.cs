using SalesforceIntegrationApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesforceIntegrationApp.Models.DTOs;


namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface IDataService
    {
        Task<List<LeadDto>> GetLeadsAsync();
        Task<List<ContactDto>> GetContactsAsync();
    }
}

