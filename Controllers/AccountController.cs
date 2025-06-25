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
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "User already exists.";
                return View();
            }
            var newUser = new User
            {
                Email = user.Email,
                Password = user.Password
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            ViewBag.Success = "Registration successful! You can now log in.";
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            var matchedUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (matchedUser != null)
            {
                HttpContext.Session.SetString("UserEmail", matchedUser.Email);
                HttpContext.Session.SetString("UserPassword", matchedUser.Password);
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.Clear();
            TempData["Error"] = "Invalid email or password";
            return View();
        }
        public IActionResult Profile()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return RedirectToAction("Login");
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
