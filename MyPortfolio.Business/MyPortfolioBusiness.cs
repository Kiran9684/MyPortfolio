using Microsoft.Extensions.Logging;
using MyPortfolio.Business.Utilities;
using MyPortfolio.DataAccess.EntityModels;
using MyPortfolio.DataAccess.Repositories;
using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.Common;
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

            var userInfo  = await _userRepository.FindUserAsync(emailAddress:loginDetail.EmailAddress, readOnly:false);

            if (userInfo is null)
            {
                businessData.IsSuccess = false;
                businessData.Errors = new List<string>() { AppMessage.ERROR_INVALID_CREDENTIALS };
                _logger.LogWarning("Login attempt failed for email: {Email}. Reason: User not found.", loginDetail.EmailAddress);
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
                        businessData.AdditionalData = new CookieInformation() { UserName = $"{userInfo.FirstName} {userInfo.LastName}", UserEmail = userInfo.Email, UserId = userInfo.Id};
                    }
                    else
                    {
                        businessData.IsSuccess = false;
                        businessData.Errors = new List<string>() { AppMessage.ERROR_USER_EXISTS_INACTIVE };
                        _logger.LogWarning("Login attempt failed for email: {Email}. Reason: Account is inactive.", loginDetail.EmailAddress);
                    }
                }
                else
                {
                    businessData.IsSuccess = false;
                    businessData.Errors = new List<string>() { AppMessage.ERROR_INVALID_CREDENTIALS };
                    _logger.LogWarning("Login attempt failed for email: {Email}. Reason: Invalid password.", loginDetail.EmailAddress);
                }
            }
            return businessData;
        }
        public async Task<BusinessData> CreateUserAccountAsync(UserRegistrationViewModel userData)
        {
            var businessData = new BusinessData();
            //1.First check if user with same email already exists, if so, throw error back to controller.
            var user = await _userRepository.FindUserAsync(emailAddress:userData.EmailAddress);

            if(user == null)
            {
                //1.Hash password with salt
                var hashedPassInfo = _passwordHasher.HashPassword(userData.Password);

                //2.Prepare User Entity from ViewModel
                string[] userFnLn = userData.FullName.Split(' ');
                var newUser = new MyPortfolio.DataAccess.EntityModels.User()
                {
                    FirstName = userFnLn[0].Trim(),
                    LastName = (userFnLn.Length > 1) ? userFnLn[1].Trim(): string.Empty,
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
                    _logger.LogError("User creation failed for email: {Email}. Reason: Database operation failed.", userData.EmailAddress);
                }
            }
            else
            {
                if (user.IsActive)
                {
                    businessData.IsSuccess = false;
                    businessData.Errors = new List<string>() { AppMessage.ERROR_USER_ALREADY_EXISTS };
                    _logger.LogWarning("User creation attempt failed for email: {Email}. Reason: Active user already exists.", userData.EmailAddress);
                }
                else
                {
                    businessData.IsSuccess = false;
                    businessData.Errors = new List<string>() { AppMessage.ERROR_USER_EXISTS_INACTIVE };
                    _logger.LogWarning("User creation attempt failed for email: {Email}. Reason: Inactive user already exists.", userData.EmailAddress);
                }
            }
            return businessData;
        }
        public async Task<BusinessData<UserProfileViewModel>> GetUserDetail(UserLoginDetail userDetail)
        {
            var data = new BusinessData<UserProfileViewModel>();

            var user = await _userRepository.FindUserAsync(emailAddress:userDetail.Email, userDetail.UserId, readOnly: true);

            if(user is not null)
            {
                data.IsSuccess = true;
                data.ViewData = new UserProfileViewModel()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Address1 = user.Address1,
                    Address2 = user.Address2 ?? string.Empty,
                    City = user.City,
                    State = user.State,
                    ZipCode = user.ZipCode,
                    IsActive = user.IsActive
                };
                
            }
            else
            {
                _logger.LogWarning("User detail retrieval failed for email: {Email}, userId: {UserId}. Reason: User not found.", userDetail.Email, userDetail.UserId);
                data.IsSuccess =false;
            }
            return data;
        }

        public async Task<BusinessData> UpdateUserProfileAsync(UserProfileViewModel userDetail, string userId)
        {
            var data = new BusinessData();

            var user = await _userRepository.FindUserAsync(userId: userId,readOnly:false);

            if(user is not null)
            {
                //manually map the properties
                user.Address1 = userDetail.Address1;
                user.Address2 = userDetail.Address2;
                user.City = userDetail.City;
                user.State = userDetail.State;
                user.ZipCode = userDetail.ZipCode;
                user.IsActive = userDetail.IsActive;
                user.FirstName = userDetail.FirstName;
                user.LastName = userDetail.LastName;

                var updatedRecordCount = await _userRepository.UpdateUserDetailsAsync(user);

                if(updatedRecordCount > 0)
                {
                    data.IsSuccess=true;
                }
                else
                {
                    data.IsSuccess = false;
                }
                _logger.LogInformation("User profile updated for userId: {UserId}. Updated record count: {UpdatedRecordCount}.", userId, updatedRecordCount);
            }
            else
            {
                data.IsSuccess=false;
                _logger.LogWarning("User profile update failed for userId: {UserId}. Reason: User not found.", userId);
            }
            return data;
        }

        public async Task<AppResponse<ProfilePictureUpdateResponse>> UpdateProfilePictureAsync(UserLoginDetail userDetail, byte[] imageData, string imageType)
        {
            if (imageData != null && imageType != null)
            {
                Guid? publicProfilePicId = null;
                var response = new ProfilePictureUpdateResponse() { ImageId = string.Empty};
                //1. read user detail
                var user = await _userRepository.FindUserAsync(userId:userDetail.UserId, emailAddress:userDetail.Email,readOnly:false, includeProfilePicture:true);

                if(user != null)
                {
                    if (user.ProfilePicture != null)
                    {
                        //over-write image info
                        publicProfilePicId = Guid.NewGuid();
                        user.ProfilePicture.ImageData = imageData;
                        user.ProfilePicture.ImageType = imageType;
                        user.ProfilePicture.PublicImageId = publicProfilePicId;
                    }
                    else
                    {
                        //add new image
                        publicProfilePicId = Guid.NewGuid();
                        var profilePic = new UserProfilePicture()
                        {
                            ImageData = imageData,
                            ImageType = imageType,
                            PublicImageId = publicProfilePicId
                        };

                        user.ProfilePicture = profilePic;
                    }

                    var updateFlag = await _userRepository.UpdateUserDetailsAsync(user);

                    if (updateFlag > 0)
                    {
                        response.ImageId = publicProfilePicId.ToString();
                        return new AppResponse<ProfilePictureUpdateResponse>(true, response);
                    }
                    else
                    {
                        return new AppResponse<ProfilePictureUpdateResponse>(false, response)
                        {
                            Errors = new List<string> { AppMessage.ERROR_PROFILE_PICTURE_SAVE_FAILED }
                        };
                    }
                }
                else
                {
                    return new AppResponse<ProfilePictureUpdateResponse>(false, null)
                    {
                        Errors = new List<string> { AppMessage.ERROR_USER_NOT_FOUND }
                    };
                }
               
            }
            else
            {
                return new AppResponse<ProfilePictureUpdateResponse>(false, null)
                {
                    Errors = new List<string> { AppMessage.ERROR_INVALID_IMAGE_PAYLOAD }
                };
            }
        }
    }
}
