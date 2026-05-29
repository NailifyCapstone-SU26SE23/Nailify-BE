using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Common
{
    public class ApiResult<T>
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        #region Empty Constructor
        public ApiResult()
        {
        }
        #endregion Empty Constructor

        public ApiResult(bool isSucceeded, string message = null)
        {
            Message = message;
            IsSucceeded = isSucceeded;
        }

        public ApiResult(bool isSucceeded, T data, string message = null)
        {
            Data = data;
            Message = message;
            IsSucceeded = isSucceeded;
        }
    }
}
