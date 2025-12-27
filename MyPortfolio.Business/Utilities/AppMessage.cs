using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Business.Utilities
{
    public static class AppMessage
    {
        public const string ERROR_SOMETHING_WENT_WRONG = "Something went wrong! Please try again later.";
        public const string ERROR_USER_ALREADY_EXISTS = "User with same email already exists. Please use different email.";
        public const string ERROR_USER_EXISTS_INACTIVE = "A user account with this email already exists but is inactive. Please contact app support for assistance.";
        public const string ERROR_INVALID_CREDENTIALS = "Invalid email or password. Please try again.";
        public const string ERROR_ACCOUNT_LOCKED = "Your account has been locked. Please contact support.";
    }
}
