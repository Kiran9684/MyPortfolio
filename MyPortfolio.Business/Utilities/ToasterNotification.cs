namespace MyPortfolio.Business.Utilities
{
    public static class ToasterNotificationTypes
    {
        public const string ERROR = "error";
        public const string INFO = "info";
        public const string SUCCESS = "success";
        public const string WARNING = "warning";
    }
    public static class ToasterNotificationMessages
    {
        public const string ERROR_DEFAULT = "Something went wrong! Please try again later.";
        public const string INFO_DEFAULT = "Operation completed successfully.";
        public const string SUCCESS_DEFAULT = "Operation completed successfully.";
        public const string WARNING_DEFAULT = "Please be cautious while performing this operation.";
        public const string ERROR_INVALID_CREDENTIALS = "Invalid Credentials. Please check your username and password.";
        public const string SUCCESS_LOGOUT = "You have been logged out successfully.";
        public const string SUCCESS_LOGIN_WELCOME = "Welcome! You have logged in successfully.";
        public const string REGESTRATION_SUCCESS = "Registration completed successfully. Please use your credentials to login.";
    }
}
