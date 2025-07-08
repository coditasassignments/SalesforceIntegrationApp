using SalesforceIntegrationApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesforceIntegrationApp.Models.DTOs;

namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface IDataService
    {
        Task<List<LeadDto>> GetLeadsAsync();
        Task<bool> UpdateLeadInSalesforceAsync(Lead lead);
        Task<bool> DeleteLeadFromSalesforceAsync(string id);
        Task<List<ContactDto>> GetContactsAsync();
        Task<bool> UpdateContactInSalesforceAsync(Contact contact);
        Task<bool> DeleteContactFromSalesforceAsync(string id);
        Task<List<CampaignDto>> GetCampaignAsync();
    }
}

