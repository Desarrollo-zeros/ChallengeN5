using Domain.Permissions;

namespace Application.Permissions.Querys
{
    public interface IGetPermissionsQueryHandler
    {
        Task<IEnumerable<PermissionResponse>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken);
    }
}