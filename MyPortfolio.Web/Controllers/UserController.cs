using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Business;
using MyPortfolio.Business.Utilities;
using MyPortfolio.Models.ViewModels;
using System.Security.Claims;

namespace MyPortfolio.Web.Controllers
{
    public class UserController : Controller
    {
        private ILogger<UserController> _logger;
        private IMyPortfolioBusiness _myPortfolioBusiness;

        public UserController(ILogger<UserController> logger, IMyPortfolioBusiness myPortfolioBusiness)
        {
            this._logger = logger;
            this._myPortfolioBusiness = myPortfolioBusiness;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginViewModel loginDetail)
        {
            if (ModelState.IsValid)
            {
                /*
                 * TODO:  
                 * business layer must return some custom response object instead of bool, like vladidation errors, is account locked, password expired etc.
                 */

                if (await _myPortfolioBusiness.ValidateUserAccount(loginDetail)) 
                {
                    await CreateAuthCookie(loginDetail);
                    TempData[ToasterNotificationTypes.SUCCESS] = ToasterNotificationMessages.SUCCESS_LOGIN_WELCOME;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //ModelState.AddModelError("loginFailure", "Invalid Login Credentials"); //This populates the validation summary in the view "asp-validation-summary"
                    TempData[ToasterNotificationTypes.ERROR] = ToasterNotificationMessages.ERROR_INVALID_CREDENTIALS;
                    return View("Index");
                }
            }
            else
            {
                TempData[ToasterNotificationTypes.ERROR] = ToasterNotificationMessages.ERROR_INVALID_CREDENTIALS; 
                return View("Index");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData[ToasterNotificationTypes.WARNING] = ToasterNotificationMessages.SUCCESS_LOGOUT;
            return RedirectToAction("Index", "User");
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RegisterUser()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserRegistrationViewModel userData)
        {
            if (!ModelState.IsValid)
            {
                return View("RegisterUser"); 
            }
            else
            {
                //2.Call BL layer to create user
                //3.In BL Layer, we nee
                //post sucess redirect to login page with success message notification
                
                TempData[ToasterNotificationTypes.SUCCESS] = ToasterNotificationMessages.REGESTRATION_SUCCESS;
                return RedirectToAction("Index", "User");
            }
        }

        [AllowAnonymous]
        public async Task<String> AccessDenied()
        {
            return "You are not authorized to access this resource.";
            //TODO: Create a view for this get a story created. 
        }

        private async Task CreateAuthCookie(LoginViewModel loginDetail)
        {
            //Step 1: Define who the user is (claims)
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginDetail.EmailAddress)
                        //new Claim(ClaimTypes.Role, "Administrator"), 
                        //coud be used for role based authorization 
                        // we can use this when we build admin panel for the application
                    };

            //Step 2: Pack claims into identity
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            /*
            //Step 3: Optional settings (remember me, expiry)
            var authProperties = new AuthenticationProperties
            {
                // IsPersistent = true;     // "Remember Me"
                // ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20);
            };
            */

            //Step 4: Create cookie and send to browser
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}
