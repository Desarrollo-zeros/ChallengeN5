using Domain.Permissions;

namespace Application.Permissions.Querys
{
    public interface IGetPermissionsFilterQueryHandler
    {
        Task<PermissionResponse> Handle(GetPermissionsFilterQuery request, CancellationToken cancellationToken);
    }
}