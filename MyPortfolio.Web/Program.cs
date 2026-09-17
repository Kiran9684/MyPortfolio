using Microsoft.AspNetCore.Authentication.Cookies;
using MyPortfolio.Business.Extensions;
using MyPortfolio.Models.AppModels;
using Serilog;
namespace MyPortfolio.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

            try
            {
                Log.Information("Starting Application");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllersWithViews();

                //Main line: Enable cookie-based authentication
                builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(
                        options =>
                        {
                            options.ExpireTimeSpan = TimeSpan.FromMinutes(2);
                            options.SlidingExpiration = true;
                            options.LoginPath = "/User/Index";  //Redirect here if user is not logged in
                            options.AccessDeniedPath = "/User/AccessDenied";  //Redirect here if user is not authorized to access a resource but logged in 
                                                                              //TODO: This functionality to be tested - Access denied path
                        }
                    ); //Handler

                // Needed if you want to access HttpContext outside controllers
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IUserFacade, UserFacade>();
                ConfigData configData = new ConfigData
                {
                    DbConnectionString = builder.Configuration.GetSection("MyPortfolioDBConfig")["ConnectionString"]
                };
                builder.Services.AddBusinessAccessServices(configData);

                builder.Services.AddSerilog((services, lc) => { lc.ReadFrom.Configuration(builder.Configuration); });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                app.UseSerilogRequestLogging();
                app.UseHttpsRedirection();
                app.UseStaticFiles();

                var cookiePolicyOptions = new CookiePolicyOptions
                {
                    MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict
                };
                app.UseCookiePolicy(cookiePolicyOptions);
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=Index}/{id?}");
                app.Run();

                Log.Information("Application Stopped Cleanly");
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
        }
    }
}
