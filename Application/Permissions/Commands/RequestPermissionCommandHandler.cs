
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
    public class RequestPermissionCommandHandler : IRequestHandler<RequestPermissionCommand, Permission>, IRequestPermissionCommandHandler
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IElasticSearchApplication<Permission> _elasticsearchService;
        private readonly IUnitOfWork _unitOfWork;

        public RequestPermissionCommandHandler(IPermissionRepository permissionRepository,
            IElasticSearchApplication<Permission> elasticsearchService, 
            IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _elasticsearchService = elasticsearchService;
            _unitOfWork = unitOfWork;
        }


        public async Task<Permission> Handle(RequestPermissionCommand command, CancellationToken cancellationToken)
        {

            // Validar que no exista otra solicitud para el mismo empleado y fecha
            var existingPermission = await _permissionRepository.GetAsync(p => p.EmployeeForename == command.EmployeeForename
                                                                          && p.EmployeSurname == command.EmployeSurname
                                                                          && p.PermissionTypeId == command.PermissionTypeId);

            if (existingPermission.Any())
            {
                throw new InvalidOperationException($"A permission request for employee '{command.EmployeeForename} {command.EmployeSurname}' on PermissionType {existingPermission?.FirstOrDefault()?.PermissionType?.Description} already exists.");
            }


            if((await _unitOfWork.PermissionTypeRepository.GetByIdAsync(command.PermissionTypeId)) == null)
            {
                throw new InvalidOperationException($"A permission Type request {command.PermissionTypeId} no exist");
            }

            var permission = new Permission
            {
                EmployeeForename = command.EmployeeForename,
                EmployeSurname = command.EmployeSurname,
                PermissionTypeId = command.PermissionTypeId,
                PermissionDate = command.PermissionDate
            };
            await _permissionRepository.AddAsync(permission);
            await _unitOfWork.CommitAsync();
            await _elasticsearchService.SaveSingleAsync(permission, permission.Id);
            return permission;
        }
    }
}
