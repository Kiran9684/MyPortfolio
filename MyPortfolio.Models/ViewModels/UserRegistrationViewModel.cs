using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.ViewModels
{
    public class UserRegistrationViewModel
    {
        [Required(ErrorMessage = "FullName Is Required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Address Is Required")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
