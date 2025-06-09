using SalesforceIntegrationApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface IOpenTaskService
    {
        Task<List<LeadAndContactWithOpenTask>> GetOpenTasksByTypeAsync(string type);
    }
}
