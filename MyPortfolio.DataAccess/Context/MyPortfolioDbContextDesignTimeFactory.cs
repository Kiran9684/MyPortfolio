using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyPortfolio.DataAccess.Context
{
    /*
     * ==============================================================================================
     * ISSUE DOCUMENTATION & RESOLUTION: Design-Time DbContext Creation in N-Tier Architecture
     * ==============================================================================================
     * * THE CONTEXT:
     * This solution follows a strict N-Tier architecture where:
     * 1. The Web Project (Startup) holds the configuration (appsettings.json).
     * 2. The DataAccess Project (Class Library) holds the DbContext and Entity models.
     * 3. Dependency Injection (DI) is handled via Extension Methods in the Business/DAL layers.
     * * THE PROBLEM:
     * When running EF Core CLI commands (e.g., 'Add-Migration'), the tools need to instantiate 
     * the 'MyPortfolioDbContext' to analyze the model schema.
     * * However, because the DI logic is encapsulated in extension methods and the DAL project 
     * is compiled separately, the EF Tools cannot automatically resolve the dependencies or locate 
     * the 'appsettings.json' file. The tools attempt to run from the DAL's output directory 
     * (bin\Debug\net8.0), resulting in a "Configuration file not found" error because they 
     * cannot see the Web project's settings.
     * * THE SOLUTION:
     * We implement the 'IDesignTimeDbContextFactory<T>' interface.
     * * This class serves as a dedicated "entry point" specifically for EF Core tools (Design Time).
     * It bypasses the application's runtime DI container entirely. Instead, it:
     * 1. Manually navigates the file system to locate the Web Project's folder.
     * 2. Manually builds the IConfiguration root to read the connection string.
     * 3. Returns a fully configured DbContext instance that the CLI tools can use.
     * * NOTE: This class is NEVER used by the running application (IIS/Production). 
     * It exists solely to unblock Migration commands in Visual Studio.
     * ==============================================================================================
     */
    public class MyPortfolioDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MyPortfolioDbContext>
    {
        public MyPortfolioDbContext CreateDbContext(string[] args)
        {
            /*
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../MyPortfolio.Web"); // this is not generating path correctly

            var config = new ConfigurationBuilder() //Creates a new instance of the configuration builder — think of it as an empty "recipe" for how to build your app’s configuration.
                .SetBasePath(basePath) //Tells the builder where to look for files (like appsettings.json).
                .AddJsonFile("appsettings.json") //Load settings from a JSON file named appsettings.json located in the base path.
                .Build(); //Actually constructs the final configuration object from all the sources you’ve added.

            //After .Build(), you get an IConfiguration object that you can use to read values
            */
            var connectionString = "Server=.;Database=Portfolio;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";//config.GetSection("MyPortfolioDBConfig")["ConnectionString"];


            var optionsBuilder = new DbContextOptionsBuilder<MyPortfolioDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MyPortfolioDbContext(optionsBuilder.Options);
        }
    }
}