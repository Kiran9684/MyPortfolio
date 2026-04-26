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

        public async Task<BusinessData> ValidateUserAccountAsync(LoginViewModel loginDetail)
        {
            var businessData = new BusinessData();

            var userInfo  = await _userRepository.FindUserByEmailAsync(loginDetail.EmailAddress, readOnly:false);

            if (userInfo is null)
            {
                businessData.IsSuccess = false;
                businessData.Errors = new List<string>() { AppMessage.ERROR_INVALID_CREDENTIALS };
            }
            else
            {
                bool passwordValidity = _passwordHasher.VerifyPassword(new HashPasswordInfo() { HashedPassword = userInfo.PasswordHash, Salt = userInfo.PasswordSalt }, loginDetail.Password);

                if (passwordValidity)
                {
                   
                    if (userInfo.IsActive)
                    {
                        await _userRepository.UpdateUserLastLoginAsync(userInfo);

                        businessData.IsSuccess = true;
                        businessData.Errors = new List<string>();
                        businessData.AdditionalData = new CookieInformation() { UserName = $"{userInfo.FirstName} {userInfo.LastName}", UserEmail = userInfo.Email };
                    }
                    else
                    {
                        businessData.IsSuccess = false;
                        businessData.Errors = new List<string>() { AppMessage.ERROR_USER_EXISTS_INACTIVE };
                    }
                }
                else
                {
                    businessData.IsSuccess = false;
                    businessData.Errors = new List<string>() { AppMessage.ERROR_INVALID_CREDENTIALS };
                }
            }
            return businessData;
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
                    IsActive = true,
                    Address1 = string.Empty,
                    City = string.Empty,
                    State = string.Empty,
                    ZipCode = string.Empty
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
