using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SalesforceIntegrationApp.Models;

namespace SalesforceIntegrationApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ||
            string.IsNullOrEmpty(HttpContext.Session.GetString("UserPassword")))
        {
            TempData["Error"] = "Please enter credentials.";
            return RedirectToAction("Login", "Account");
        }
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
