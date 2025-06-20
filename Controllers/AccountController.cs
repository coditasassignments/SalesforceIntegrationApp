using Microsoft.AspNetCore.Mvc;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Data;
using System.Linq;

namespace SalesforceIntegrationApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string email, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                ViewBag.Error = "User already exists.";
                return View();
            }

            var newUser = new User
            {
                Email = email,
                Password = password // for real apps, hash this!
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            ViewBag.Success = "Registration successful! You can now log in.";
            return View();
        }


        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid user";
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
