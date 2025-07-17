using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SalesforceIntegrationApp.Data;
using SalesforceIntegrationApp.Services.Implementations;
using SalesforceIntegrationApp.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
//builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ISalesforceService, SalesforceIntegrationApp.Services.Implementations.SalesforceService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IInProgressService, InProgressService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"C:\Keys\"))
    .SetApplicationName("SalesforceIntegrationApp");
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.Run();
