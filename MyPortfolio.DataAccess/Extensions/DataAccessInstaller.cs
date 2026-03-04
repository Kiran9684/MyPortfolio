using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyPortfolio.DataAccess.Context;
using MyPortfolio.Models.AppModels;

namespace MyPortfolio.DataAccess.Extensions
{
    public static class DataAccessInstaller
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, ConfigData configData)
        {
            //DO DI registration for DbContext here.
            services.AddDbContext<MyPortfolioDbContext>(dbOptions =>
            {
                dbOptions.UseSqlServer(configData.DbConnectionString); //TODO:We cant keep the connection string in appsettings.json file. We need to use Azure Key Vault or some other secure way to store the connection string. For now, we will keep it in appsettings.json file for simplicity.
            });

            //Do DI registration for other DAL repositories here.
            services.AddScoped<MyPortfolio.DataAccess.Repositories.IUserRepository, MyPortfolio.DataAccess.Repositories.UserRepository>();

            return services;
        }
    }
}
