using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Business;
using MyPortfolio.Business.Utilities;
using MyPortfolio.Models.AppModels;
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
                var businessData = await _myPortfolioBusiness.ValidateUserAccountAsync(loginDetail);
                if (businessData.IsSuccess) 
                {
                    await CreateAuthCookie((CookieInformation)businessData.AdditionalData); 
                    TempData[ToasterNotificationTypes.SUCCESS] = ToasterNotificationMessages.SUCCESS_LOGIN_WELCOME;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData[ToasterNotificationTypes.ERROR] = businessData.Errors[0];
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
                var busineesData = await _myPortfolioBusiness.CreateUserAccountAsync(userData);

                if (busineesData.IsSuccess)
                {
                    TempData[ToasterNotificationTypes.SUCCESS] = ToasterNotificationMessages.REGESTRATION_SUCCESS;
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach(var error in busineesData.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error); //This populates the validation summary in the view "asp-validation-summary"
                        //The empty string(string.Empty) tells MVC that this error is not associated with any particular field / property.
                        //Instead, it becomes a model‑level error.
                    }
                    return View("RegisterUser");
                }
            }
        }

        [AllowAnonymous]
        public async Task<String> AccessDenied()
        {
            return await Task.FromResult("You are not authorized to access this resource.");
            //TODO: Create a view for this get a story created. 
        }


        [Authorize]
        public IActionResult UserProfile()
        {
            //Here you should read user details from request, then using the detail like user email , call db and pull complete user details
            //populate the view model and pass to view, for now I am hardcoding the values. 
            //In future we shall store the data in session for the first time when user logins and then we can read the data from session to populate the view model.
            var model = new UserProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "kirankumarbs1034@gmail.com",
                Address1 = "123 Main St",
                Address2 = "Apt 4B",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345",
                IsActive = true
            };
            return View(model);
        }

        [Authorize]
        public IActionResult SaveUserProfile(UserProfileViewModel usrProfileDetails)
        {
            if (ModelState.IsValid)
            {
                //First update the user details in db, then redirect to user profile page, so that we can see the updated details in view, here we are not saving the data to db, so we will not see any changes in the view.
                //Display toast message to mention that user details updated successfully. 
                return RedirectToAction("UserProfile");
                //By redirecting to action, we will get the latest data from db and display to user, here we are not saving the data to db, so we will not see any changes in the view.
            }
            else
            {
                return View("UserProfile", usrProfileDetails);
            }
        }

        private async Task CreateAuthCookie(CookieInformation info)
        {
            //Step 1: Define who the user is (claims)
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, info.UserName),
                        new Claim(ClaimTypes.Email, info.UserEmail)
                        //new Claim(ClaimTypes.Role, "Administrator"), //coud be used for role based authorization we can use this when we build admin panel for the application
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
