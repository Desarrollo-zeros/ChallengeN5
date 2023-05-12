using Domain.Permissions;

namespace Application.Permissions.Commands
{
    public interface IRequestPermissionCommandHandler
    {
        Task<Permission> Handle(RequestPermissionCommand command, CancellationToken cancellationToken);
    }
}