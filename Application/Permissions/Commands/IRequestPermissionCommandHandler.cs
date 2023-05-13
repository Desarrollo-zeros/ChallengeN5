
using Domain.Permissions;

namespace Application.Permissions.Commands
{
    public interface IRequestPermissionCommandHandler
    {
        Task<PermissionResponse> Handle(RequestPermissionCommand command, CancellationToken cancellationToken);
    }
}