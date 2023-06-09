using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Common.ServiceResponse
{
    public class BaseResponse
    {
        public BaseResponse()
        {
        }
        public BaseResponse(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }

    }
}
