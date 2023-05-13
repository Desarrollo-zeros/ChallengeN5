using Domain.Permissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions.Querys
{
    public record GetPermissionsQuery() : IRequest<IEnumerable<PermissionResponse>>;

    public record GetPermissionsFilterQuery(int id) : IRequest<PermissionResponse>;

}
