using MyPortfolio.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Business.Utilities
{
    public interface IPasswordHasher
    {
        public HashPasswordInfo HashPassword(string password);
        public bool VerifyPassword(HashPasswordInfo hashedPasswordWithSalt, string providedPassword);
    }
}
