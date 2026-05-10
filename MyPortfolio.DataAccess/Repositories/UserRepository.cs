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

        public async Task<User> FindUserAsync(string? emailAddress = null,string? userId = null,bool readOnly = true)
        {
            IQueryable<User> query = _dbContext.Users;
            User? user = null;
            if(readOnly)
            {
                query = query.AsNoTracking();
            }
           
            if(userId is not null)
            {
                //string[] userFnLn = userName.Split(' ');
                user = await query.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            }
            else
            {
                user = await query.FirstOrDefaultAsync(u => u.Email == emailAddress);
            }

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

        public async Task<int> UpdateUserDetails(User user)
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
