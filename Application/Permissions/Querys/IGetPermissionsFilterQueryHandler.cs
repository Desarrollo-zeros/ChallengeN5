using Domain.Permissions;

namespace Application.Permissions.Querys
{
    public interface IGetPermissionsFilterQueryHandler
    {
        Task<Permission?> Handle(GetPermissionsFilterQuery request, CancellationToken cancellationToken);
    }
}