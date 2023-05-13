using Domain.Permissions;
using Infrastructure.Audilog;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logs.Querys
{
    public record GetLogsQuery() : IRequest<IEnumerable<LogDto>>;

    

}
