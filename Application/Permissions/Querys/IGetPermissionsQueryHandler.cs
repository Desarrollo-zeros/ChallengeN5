using Domain.Permissions;

namespace Application.Permissions.Querys
{
    public interface IGetPermissionsQueryHandler
    {
        Task<IEnumerable<Permission>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken);
    }
}