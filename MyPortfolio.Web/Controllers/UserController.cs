using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Web.Models;

namespace MyPortfolio.Web.Controllers
{
    public class UserController : Controller
    {
        private ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            this._logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate(LoginViewModel loginDetail)
        {
            //TODO: Need To Implement Cookie Based Authentication

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Index");
            }
        }
    }
}
