using Domain.Interface.Permissions;
using Domain.Permissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions.Querys
{
    public class GetPermissionsFilterQueryHandler : IRequestHandler<GetPermissionsFilterQuery, Permission?>, IGetPermissionsFilterQueryHandler
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionsFilterQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<Permission?> Handle(GetPermissionsFilterQuery request, CancellationToken cancellationToken)
        {
            return await _permissionRepository.GetPermissionByIdAsync(request.id);
        }
    }
}
