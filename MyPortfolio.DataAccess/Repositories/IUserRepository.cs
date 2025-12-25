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
        public Task<User> FindUserByEmailAsync(string emailAddress);
        public Task<bool> CreateUserAsync(User newUser);
    }
}
