using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.Common;

namespace MyPortfolio.Web.Facades
{
    public interface IUserFacade
    {
        public Task<(ValidationResult validation, byte[]? imageData, string? imageType)> ValidateProfilePictureAsync(IFormFile file, ProfilePictureSettings settings);
    }
}
