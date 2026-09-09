using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.ViewModels
{
    public class ProfilePictureUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }
    }
}
