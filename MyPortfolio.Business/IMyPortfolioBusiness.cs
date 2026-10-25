using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.Common;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Business
{
    public interface IMyPortfolioBusiness
    {
        public Task<BusinessData> ValidateUserAccountAsync(LoginViewModel loginDetail);

        public Task<BusinessData> CreateUserAccountAsync(UserRegistrationViewModel userData);

        public Task<BusinessData<UserProfileViewModel>> GetUserDetail(UserLoginDetail userDetail);

        public Task<BusinessData> UpdateUserProfileAsync(UserProfileViewModel userDetail, string userId);

        public Task<AppResponse<ProfilePictureUpdateResponse>> UpdateProfilePictureAsync(UserLoginDetail userDetail, byte[] imageData, string imageType);

        public Task<AppResponse<UserImage>> GetUserProfilePictureAsync(string userId, Guid publicUsrImageId);
    }
}
