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
        
        //[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        //[HttpPost]
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
                    _logger.LogInformation("User Authentication successful for the user : {Email}", loginDetail.EmailAddress);
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
                _logger.LogWarning("User Authentication failed due to invalid model state for the user : {Email}", loginDetail.EmailAddress);
                TempData[ToasterNotificationTypes.ERROR] = ToasterNotificationMessages.ERROR_INVALID_CREDENTIALS; 
                return View("Index");
            }
        }

        [Authorize]
        //[HttpGet]
        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData[ToasterNotificationTypes.WARNING] = ToasterNotificationMessages.SUCCESS_LOGOUT;
            _logger.LogInformation("User signed out successfully.");
            return RedirectToAction("Index", "User");
        }

        [AllowAnonymous]
        //[HttpGet]
        public async Task<IActionResult> RegisterUser()
        {
            return View();
        }

        //[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserRegistrationViewModel userData)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("User registration failed due to invalid model state for the user : {Email}", userData.EmailAddress);
                return View("RegisterUser"); 
            }
            else
            {
                var busineesData = await _myPortfolioBusiness.CreateUserAccountAsync(userData);

                if (busineesData.IsSuccess)
                {
                    TempData[ToasterNotificationTypes.SUCCESS] = ToasterNotificationMessages.REGESTRATION_SUCCESS;
                    _logger.LogInformation("User registration successful for the user : {Email}", userData.EmailAddress);
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
        public async Task<IActionResult> UserProfile()
        {
            
            UserProfileViewModel viewModel;
            //1.Read user email id and name from auth cookie. 

            //Approach 01:
            //var claimsIdentities = User.Identities.ToList();
            //claimsIdentities.ForEach(claimIdentiy =>
            //{
            //    foreach (var claim in claimIdentiy.Claims)
            //    {
            //        if(claim.Type == ClaimTypes.Name)
            //        {
            //            user.UserName = claim.Value.ToString();
            //        }

            //        if(claim.Type == ClaimTypes.Email)
            //        {
            //            user.Email = claim.Value.ToString();
            //        }
            //    }
            //});

            //Approach 02:
            var user = ReadUserInfoFromAuthCookie();

            //2.Business layer call to get user view model.
            var businessData = await _myPortfolioBusiness.GetUserDetail(user);
            if (businessData.IsSuccess)
            {
                viewModel = businessData.ViewData;
                _logger.LogInformation("User details fetched successfully for the user : {Email}", user.Email);
                return View(viewModel);
            }
            else
            {
                TempData[ToasterNotificationTypes.ERROR] = ToasterNotificationMessages.USERDETAILS_FETCH_ERROR;
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> SaveUserProfile(UserProfileViewModel usrProfileDetails)
        {
            if (ModelState.IsValid)
            {
                var userLoginInfo = ReadUserInfoFromAuthCookie();
                var data = await _myPortfolioBusiness.UpdateUserProfileAsync(usrProfileDetails,userLoginInfo.UserId);

                if (data.IsSuccess)
                {
                    TempData[ToasterNotificationTypes.SUCCESS] = ToasterNotificationMessages.USERDETAILS_UPDATE_SUCCESS;
                    return RedirectToAction("UserProfile");
                }
                else
                {
                    TempData[ToasterNotificationTypes.SUCCESS] = ToasterNotificationMessages.USERDETAILS_UPDATE_ERROR; ;
                    return View("UserProfile", usrProfileDetails);
                }
            }
            else
            {
                _logger.LogWarning("User profile update failed due to invalid model state for the user : {Email}", usrProfileDetails.Email);
                return View("UserProfile", usrProfileDetails);
            }
        }

        private async Task CreateAuthCookie(CookieInformation info)
        {
            //Step 1: Define who the user is (claims)
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, info.UserName),
                        new Claim(ClaimTypes.Email, info.UserEmail),
                        new Claim(ClaimTypes.NameIdentifier, info.UserId.ToString())
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

            //Step05: 
            //In future we shall store the data in session for the first time when user logins and then we can read the data from session to populate the view models wherever user info is needed.
        }

        private UserLoginDetail ReadUserInfoFromAuthCookie()
        {
            var user = new UserLoginDetail();
            user.UserName = User.FindFirstValue(ClaimTypes.Name);
            user.Email = User.FindFirstValue(ClaimTypes.Email);
            user.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return user;

            //Need to to inject this as a dependency inject from DI container. 
        }
    }
}
