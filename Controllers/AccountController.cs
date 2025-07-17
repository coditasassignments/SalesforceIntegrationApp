using Microsoft.AspNetCore.Mvc; //is a core namespace that gives MVC Model-View-Controller applications
using SalesforceIntegrationApp.Models; //is a namespace that gives access to all the classes of Models folder
using SalesforceIntegrationApp.Data; //is a namespace that gives access to Data folder to use database
using System.Linq; //this namespace is imported inorder to use properties of Linq: FirstOrDefault 
using SalesforceIntegrationApp.Logging; //inorder to add logger in the file
using Microsoft.AspNetCore.Authorization;

namespace SalesforceIntegrationApp.Controllers //importing namespace 
{
    public class AccountController : Controller //class AccountController inheriting from base controller class
    {
        private readonly ApplicationDbContext _context; //declaring private readonly to hold the reference of ApplicationDbContext 
        public AccountController(ApplicationDbContext context) //constructor of the AccountController class 
        {
            _context = context; // variable that will reference the database.
        }
        [HttpGet]
        public IActionResult Register() // When user clicks on register the control passes to this action
        {
            Logger.LogInfo("Register accessed"); // added logger functionality inorder to keep the track of application
            return View(); //returns the register view with empty fields
        }
        [HttpPost]
        public IActionResult Register(User user) // When user completely fills the details, clicks on register then this action gets triggered
        {
            Logger.LogInfo("Register triggered"); // keeps track
            if (!ModelState.IsValid) // if the values passed in the model are not valid, this block gets executed 
            {
                Logger.LogInfo("Model state is not valid during registration"); // keeps track
                return View(user); // again returns the view
            }
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email); // if the model passed in the action is valid, then the details will be stored in the database
                if (existingUser != null) // checks the database, if the user already exists in the database, gives error.
                {
                    Logger.LogInfo("Registration failed");
                    ViewBag.Error = "User already exists."; // gives the error
                    return View(); // returns the same register view 
                }
                var newUser = new User // otherwise creates a new object with the email and password
                {
                    Email = user.Email, 
                    Password = user.Password
                };
                _context.Users.Add(newUser); // adds the new user to the database
                _context.SaveChanges(); // makes changes in the database
                Logger.LogInfo("User registered successfully");
                ViewBag.Success = "Registration successful! You can now log in"; //gives the success message
                return View(); // returns the view
            }
            catch (Exception ex) //if try block fails, control gets into the catch block
            {
                Logger.LogError("Exception occurred during registration.", ex);
                ViewBag.Error = "An error occurred.Please try again."; // gives error
                return View(user); // returns view
            }
        }
        [HttpGet]
        public IActionResult Login() // The user is at first directed to the login page
        {
            Logger.LogInfo("Login accessed");
            HttpContext.Session.Clear(); // before logging in, clears the session
            return View(); // returns the view 
        }
        [HttpPost]
        public IActionResult Login(User user) // When user fill all the login details 
        {
            try
            {
                var matchedUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password); // checks in the users table if the user is already registered or not 
                if (matchedUser != null) // if the user gets matched with the registered email and password
                {
                    HttpContext.Session.SetString("UserEmail", matchedUser.Email); // the session gets set with the user Email
                    HttpContext.Session.SetString("UserPassword", matchedUser.Password); // the session gets set with the user password
                    Logger.LogInfo($"User logged in successfully: {user.Email}");
                    return RedirectToAction("Index", "Home"); // and directed towards the main dashboard
                }
                Logger.LogInfo($"Login failed for email: {user.Email}");
                HttpContext.Session.Clear(); // if login failed, clears the session
                TempData["Error"] = "Invalid email or password"; // gives error message
                return View(); //and returns the view
            }
            catch (Exception ex) // if the try block fails, the control passes towards the catch block
            {
                Logger.LogError("Exception occurred during login.", ex);
                TempData["Error"] = "An error occurred. Please try again."; //gives the error
                return View(); // returns the view
            }
        }
        public IActionResult Profile() // this is the profile action
        {
            var email = HttpContext.Session.GetString("UserEmail"); // getting the "userEmail"(key) that is stored in the session
            Logger.LogInfo("Accessing profile");
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email); // checking the database if the user exists or not
                if (user == null) // if user does not exist
                {
                    Logger.LogInfo("Error, user not found!");
                    return RedirectToAction("Login"); // redirects to the login page
                }
                return View(user);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred while loading profile.", ex);
                return RedirectToAction("Login");
            }
        }

        public IActionResult Logout() // when user try to logout,this action gets triggered
        {
            Logger.LogInfo($"User logging out");
            HttpContext.Session.Clear(); // clears the active session
            return RedirectToAction("Login"); // redirects to the login action 
        }
    }
}
//string query = "SELECT Id, Subject, Status, ActivityDate, Description, Priority FROM Task";

