using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email Address Is Required")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "User Password Is Required")]
        [DataType(DataType.Password)] //This is need to set type attribute of input html tag to password
        public string Password { get; set; }
    }
}
