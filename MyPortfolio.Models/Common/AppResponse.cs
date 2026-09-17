using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.Common
{
    public class AppResponse<T>
    {
        public bool IsSucess {  get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public AppResponse(bool isSucess, T Data) 
        {
            this.IsSucess = isSucess;
            this.Data = Data;
        }
    }
}
