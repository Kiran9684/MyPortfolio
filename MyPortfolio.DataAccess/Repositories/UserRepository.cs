using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyPortfolio.DataAccess.Context;
using MyPortfolio.DataAccess.EntityModels;

namespace MyPortfolio.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyPortfolioDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(MyPortfolioDbContext dbContext, ILogger<UserRepository> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<User> FindUserByEmailAsync(string emailAddress)
        {
           var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailAddress);
           return user;
        }

        public async Task<bool> CreateUserAsync(User newUser)
        {
           _dbContext.Users.Add(newUser);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
