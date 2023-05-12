using Domain.Permissions;

namespace Application.Permissions.Commands
{
    public interface IModifyPermissionCommandHandler
    {
        Task<Permission> Handle(ModifyPermissionCommand command, CancellationToken cancellationToken);
    }
}