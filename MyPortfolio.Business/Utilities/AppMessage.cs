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
        public const string ERROR_USER_NOT_FOUND = "User account could not be located.";
        public const string ERROR_PROFILE_PICTURE_SAVE_FAILED = "Failed to commit the profile picture update to the database. Please try again.";
        public const string ERROR_INVALID_IMAGE_PAYLOAD = "The provided image data or format type is missing.";
        public const string ERROR_PROFILE_PICTURE_INVALID_EXTENSION ="Only PNG and JPEG/JPG image formats are supported.";
        public const string ERROR_PROFILE_PICTURE_SIZE_EXCEEDED ="Profile picture size must not exceed 2 MB.";
        public const string ERROR_PROFILE_PICTURE_INVALID_CONTENT ="The uploaded file is not a valid image.";
        public const string ERROR_PROFILE_PICTURE_NOT_FOUND ="Profile picture could not be located.";
    }
}
