using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyPortfolio.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                    }
                ); //Handler

            // Needed if you want to access HttpContext outside controllers
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<MyPortfolio.Business.IMyPortfolioBusiness, MyPortfolio.Business.MyPortfolioBusiness>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

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
        }
    }
}
