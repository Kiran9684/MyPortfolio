using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Business
{
    public interface IMyPortfolioBusiness
    {
        public Task<BusinessData> ValidateUserAccountAsync(LoginViewModel loginDetail);

        public Task<BusinessData> CreateUserAccountAsync(UserRegistrationViewModel userData);
    }
}
