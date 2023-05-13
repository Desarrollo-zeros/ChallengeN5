using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Responses
{
    public interface IResponse<T> where T : class
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Entity { get; set; }
    }
}
