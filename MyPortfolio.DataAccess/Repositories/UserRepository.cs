using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyPortfolio.DataAccess.Context;
using MyPortfolio.DataAccess.EntityModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<User> FindUserByEmailAsync(string emailAddress, bool readOnly = true)
        {
            IQueryable<User> query = _dbContext.Users;

            if(readOnly)
            {
                query = query.AsNoTracking();
            }

            var user = await query.FirstOrDefaultAsync(u => u.Email == emailAddress);
           
            return user;
        }

        public async Task<bool> CreateUserAsync(User newUser)
        {
           _dbContext.Users.Add(newUser);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task UpdateUserLastLoginAsync(User user)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }
}
