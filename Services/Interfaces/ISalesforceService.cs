﻿using SalesforceIntegrationApp.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Interfaces
{
    public interface ISalesforceService
    {
        //Task<List<LeadFieldDto>> GetLeadFieldsAsync(string accessToken, string instanceUrl);
        Task<List<dynamic>> GetLeadFieldsAsync(string accessToken, string instanceUrl);

    }
}
