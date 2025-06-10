using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesforceIntegrationApp.Services.Implementations
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<SalesforceAuth> GetValidTokenAsync()
        {
            var credentials = _context.SalesforceAuth.FirstOrDefault();
            if (credentials == null || string.IsNullOrEmpty(credentials.AccessToken) || string.IsNullOrEmpty(credentials.InstanceUrl))
            {
                throw new Exception("Not found");
            }
            return credentials;
        }
    }
}
