using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface IInProgressService
    {
        Task<List<LeadInProgressDto>> GetLeadInProgressAsync();
        Task<List<ContactInProgressDto>> GetContactInProgressAsync();
    }
}
