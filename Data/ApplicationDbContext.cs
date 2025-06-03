using Microsoft.EntityFrameworkCore;
using SalesforceIntegrationApp.Models;

namespace SalesforceIntegrationApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lead> Leads { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<LeadAndContactWithOpenTask> LeadAndContactWithOpenTasks { get; set; }


    }
}
