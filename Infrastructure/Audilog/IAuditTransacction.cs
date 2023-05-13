using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Audilog
{
    public interface IAuditTransacction
    {
        string ActionName { get; set; }
        Task StartLog(HttpContext httpContext);
        Task EndLog(HttpContext httpContext);
       
    }
}
