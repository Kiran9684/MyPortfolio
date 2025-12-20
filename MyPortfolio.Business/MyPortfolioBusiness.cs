using Microsoft.Extensions.Logging;
using MyPortfolio.Business.Utilities;
using MyPortfolio.DataAccess.Repositories;
using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Business
{
    public class MyPortfolioBusiness : IMyPortfolioBusiness
    {
        private readonly ILogger<MyPortfolioBusiness> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public MyPortfolioBusiness(ILogger<MyPortfolioBusiness> logger, IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this._logger = logger;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> ValidateUserAccountAsync(LoginViewModel loginDetail)
        {

            /*
             TODO: 
                -- Validate user credentials against database
                -- If valid, proceed to create claims and sign in
                    --hash the password with salt and compare with the database stored hash password
                -- If invalid, return to login view with error message

                --Keep seperate models for loginview and actual user model for database. 
                --Append LastUserModified info into claims for reaction to the backend changes. 
            */
            await Task.Delay(500); 

            if(loginDetail.EmailAddress == "admin@gmail.com")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<BusinessData> CreateUserAccountAsync(UserRegistrationViewModel userData)
        {
            var businessData = new BusinessData();
            //1.First check if user with same email already exists, if so, throw error back to controller.
            var user = await _userRepository.FindUserByEmailAsync(userData.EmailAddress);

            if(user == null)
            {
                //1.Hash password with salt
                var hashedPassInfo = _passwordHasher.HashPassword(userData.Password);

                //2.Prepare User Entity from ViewModel
                string[] userFnLn = userData.FullName.Split(' ');
                var newUser = new MyPortfolio.DataAccess.EntityModels.User()
                {
                    FirstName = userFnLn[0],
                    LastName = (userFnLn.Length > 1) ? userFnLn[1]: string.Empty,
                    Email = userData.EmailAddress,
                    PasswordHash = hashedPassInfo.HashedPassword,
                    PasswordSalt = hashedPassInfo.Salt,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                //3.Save to database using repository
                var userCreation =  await _userRepository.CreateUserAsync(newUser);

                //4. check flag Return success/failure(due to exception) response to controller
                if (userCreation)
                {
                    businessData.IsSuccess = true;
                    businessData.Errors = new List<string>();
                }
                else
                {
                    businessData.IsSuccess = false;
                    businessData.Errors = new List<string>() {AppMessage.ERROR_SOMETHING_WENT_WRONG};
                }
            }
            else
            {
                if (user.IsActive)
                {
                    businessData.IsSuccess = false;
                    businessData.Errors = new List<string>() { AppMessage.ERROR_USER_ALREADY_EXISTS };
                }
                else
                {
                    businessData.IsSuccess = false;
                    businessData.Errors = new List<string>() { AppMessage.ERROR_USER_EXISTS_INACTIVE };
                }
            }
            return businessData;
        }
    }
}
