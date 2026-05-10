using MyPortfolio.DataAccess.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.DataAccess.Repositories
{
    public interface IUserRepository
    {
        public Task<User> FindUserAsync(string emailAddress = null,string? userId =null, bool readOnly = true);
        public Task<bool> CreateUserAsync(User newUser);
        public Task UpdateUserLastLoginAsync(User user);
        public Task<int> UpdateUserDetails(User user);
    }
}
