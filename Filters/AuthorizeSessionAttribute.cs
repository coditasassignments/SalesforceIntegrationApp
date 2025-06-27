using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SalesforceIntegrationApp.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute //custom class to check if the user is logged in or not
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session; //checking the session object
            var u = session.GetString("UserEmail"); //gets the email from the session object
            if (string.IsNullOrEmpty(u)) // if email not present 
            {
                context.Result = new RedirectToActionResult("Login", "Account", null); //redirect to login action of the Account Controller
            }
        }
    }
}
