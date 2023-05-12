using Domain.Exceptions;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Domain.Permissions;
using Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions.Commands
{
    public class ModifyPermissionCommandHandler : IRequestHandler<ModifyPermissionCommand, Permission>, IModifyPermissionCommandHandler
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IElasticSearchApplication<Permission> _elasticsearchService;
        private readonly IUnitOfWork _unitOfWork;

        public ModifyPermissionCommandHandler(IPermissionRepository permissionRepository,
            IElasticSearchApplication<Permission> elasticsearchService, 
            IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _elasticsearchService = elasticsearchService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Permission> Handle(ModifyPermissionCommand command, CancellationToken cancellationToken)
        {

            var permission = await _permissionRepository.GetByIdAsync(command.Id);

            if (permission == null)
                throw new NotFoundException(nameof(Permission), command.Id);
            var l = (await _unitOfWork.PermissionTypeRepository.GetByIdAsync(command.PermissionTypeId));
            if (l == null)
            {
                throw new InvalidOperationException($"A permission Type request {command.PermissionTypeId} no exist");
            }

            permission.EmployeeForename = command.EmployeeForename;
            permission.EmployeSurname = command.EmployeSurname;
            permission.PermissionTypeId = command.PermissionTypeId;
            permission.PermissionDate = command.PermissionDate;

            _permissionRepository.Update(permission);
            await _unitOfWork.CommitAsync();
            await _elasticsearchService.SaveSingleAsync(permission, permission.Id);
            return permission;
        }
    }
}
