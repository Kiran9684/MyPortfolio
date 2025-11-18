using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Business
{
    public interface IMyPortfolioBusiness
    {
        public Task<bool> ValidateUserAccount(LoginViewModel loginDetail);
    }
}
