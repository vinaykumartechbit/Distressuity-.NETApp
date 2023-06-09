using DISTRESSUITY.Common.ServiceResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Common.Service_Response
{
    public class AuthenticationServiceResponse : BaseResponse
    {
        public Object Data { get; set; }

        public List<Object> MultipleData { get; set; }

        public ClaimsIdentity identity { get; set; }
    }
}
