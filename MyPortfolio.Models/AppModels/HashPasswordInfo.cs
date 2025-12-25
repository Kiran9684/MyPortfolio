using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.AppModels
{
    public class HashPasswordInfo
    {
        public string HashedPassword { get; set; } 
        public string Salt { get; set; }
    }
}
