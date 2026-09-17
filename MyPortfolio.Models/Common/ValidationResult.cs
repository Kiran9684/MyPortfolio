using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.Common
{
    public class ValidationResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
    }
}
