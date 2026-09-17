using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.AppModels
{
    public class ProfilePictureSettings
    {
        public List<string> PermittedExtensions { get; set; }
        public int SizeInBytes {  get; set; }
    }
}
