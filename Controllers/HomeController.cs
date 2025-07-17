using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Logging;
using SalesforceIntegrationApp.Filters;
using Microsoft.AspNetCore.Authorization;

namespace SalesforceIntegrationApp.Controllers;

[AuthorizeSession]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        Logger.LogInfo("/Home/Index accessed.");
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ||
            string.IsNullOrEmpty(HttpContext.Session.GetString("UserPassword")))
        {
            Logger.LogInfo("Session missing.Redirecting to /Account/Login");
            TempData["Error"] = "Please enter credentials.";
            return RedirectToAction("Login", "Account");
        }
        Logger.LogInfo("User session valid.Displaying Index view");
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
