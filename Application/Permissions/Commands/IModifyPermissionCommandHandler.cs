
using Domain.Permissions;

namespace Application.Permissions.Commands
{
    public interface IModifyPermissionCommandHandler
    {
        Task<PermissionResponse> Handle(ModifyPermissionCommand command, CancellationToken cancellationToken);
    }
}