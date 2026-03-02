using Microsoft.AspNetCore.Mvc;

namespace MyPortfolio.Web.ViewComponents
{
    public class UserProfileDropdownViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInUserName = User.Identity?.Name;

            if(loggedInUserName != null)
            {
                ViewData["userName"] = loggedInUserName;  
            }
            else
            {
                //TODO:If data is not available then read from session/db call (this will be decided later) and set it in view data.
                ViewData["userName"] = "Guest"; // Fallback value if user is not authenticated
            }

            return View(); 
        }
    }
}
