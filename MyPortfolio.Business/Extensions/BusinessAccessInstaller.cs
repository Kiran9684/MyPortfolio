using Microsoft.Extensions.DependencyInjection;
using MyPortfolio.DataAccess.Extensions;
using MyPortfolio.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Business.Extensions
{
    public static class BusinessAccessInstaller
    {
        public static IServiceCollection AddBusinessAccessServices(this IServiceCollection services, ConfigData configData)
        {
            #region Business Layer Registrations
            services.AddScoped<MyPortfolio.Business.IMyPortfolioBusiness, MyPortfolio.Business.MyPortfolioBusiness>();
            services.AddScoped<MyPortfolio.Business.Utilities.IPasswordHasher, MyPortfolio.Business.Utilities.PasswordHasher>();
            #endregion

            #region DAL Layer Registrations
            services.AddDataAccessServices(configData);
            #endregion

            return services;
        }
    }
}
