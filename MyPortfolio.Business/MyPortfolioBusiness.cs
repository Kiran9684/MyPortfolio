using Microsoft.Extensions.Logging;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Business
{
    public class MyPortfolioBusiness : IMyPortfolioBusiness
    {
        private readonly ILogger<MyPortfolioBusiness> _logger;
        public MyPortfolioBusiness(ILogger<MyPortfolioBusiness> logger)
        {
            this._logger = logger;
        }

        public async Task<bool> ValidateUserAccount(LoginViewModel loginDetail)
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
    }
}
