using Microsoft.AspNetCore.Mvc;
using SalesforceIntegrationApp.Models;
using SalesforceIntegrationApp.Data;
using System.Linq;
using SalesforceIntegrationApp.Logging;

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
            Logger.LogInfo("Register accessed");
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            Logger.LogInfo("Register triggered");
            if (!ModelState.IsValid)
            {
                Logger.LogInfo("Model state is not valid during registration");
                return View(user);
            }
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    Logger.LogInfo("Registration failed");
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
                Logger.LogInfo("User registered successfully");
                ViewBag.Success = "Registration successful! You can now log in";
                return View();
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred during registration.", ex);
                ViewBag.Error = "An error occurred.Please try again.";
                return View(user);
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            Logger.LogInfo("Login accessed");
            HttpContext.Session.Clear();
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            Logger.LogInfo($"Login triggered: {user.Email}");
            try
            {
                var matchedUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (matchedUser != null)
                {
                    HttpContext.Session.SetString("UserEmail", matchedUser.Email);
                    HttpContext.Session.SetString("UserPassword", matchedUser.Password);
                    Logger.LogInfo($"User logged in successfully: {user.Email}");
                    return RedirectToAction("Index", "Home");
                }
                Logger.LogInfo($"Login failed for email: {user.Email}");
                HttpContext.Session.Clear();
                TempData["Error"] = "Invalid email or password";
                return View();
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred during login.", ex);
                TempData["Error"] = "An error occurred. Please try again.";
                return View();
            }
        }
        public IActionResult Profile()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            Logger.LogInfo("Accessing profile");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    Logger.LogInfo("Error, user not found!");
                    return RedirectToAction("Login");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred while loading profile.", ex);
                return RedirectToAction("Login");
            }
        }

        public IActionResult Logout()
        {
            Logger.LogInfo($"User logging out");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
