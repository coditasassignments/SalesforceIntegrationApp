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
        public DbSet<User> Users { get; set; }
        public DbSet<SalesforceAuth> SalesforceAuth { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<LeadAndContactWithOpenTask> LeadAndContactWithOpenTasks { get; set; }
        public DbSet<ReportData> ReportDatas { get; set; }
        public DbSet<LeadInProgress> LeadsInProgress { get; set; }
        public DbSet<ContactInProgress> ContactsInProgress { get; set; }
        public DbSet<LeadOpenActivity> LeadsOpenActivity { get; set; }


    }
}
