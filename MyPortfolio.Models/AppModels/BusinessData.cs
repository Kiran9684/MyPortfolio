using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.AppModels
{
    public class BusinessData<T>
    {
        public T ViewData { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Object ? AdditionalData { get; set; }
    }

    public class BusinessData
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Object? AdditionalData { get; set; }
    }
}
