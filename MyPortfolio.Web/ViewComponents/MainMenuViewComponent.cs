using Microsoft.AspNetCore.Mvc;

namespace MyPortfolio.Web.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // You can add any logic here to prepare data for the view if needed
            return View(); // This will look for a view named "Default.cshtml" in the Views/Shared/Components/MainMenu folder
        }
    }
}
