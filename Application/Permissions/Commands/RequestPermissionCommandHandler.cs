
using AutoMapper;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Domain.Permissions;
using Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Permissions.Commands
{
    public class RequestPermissionCommandHandler : IRequestHandler<RequestPermissionCommand, PermissionResponse>, IRequestPermissionCommandHandler
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IElasticSearchApplication<PermissionElasticsearch> _elasticsearchService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RequestPermissionCommandHandler> _logger;
        public RequestPermissionCommandHandler(IPermissionRepository permissionRepository,
            IElasticSearchApplication<PermissionElasticsearch> elasticsearchService, 
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RequestPermissionCommandHandler> logger)
        {
            _permissionRepository = permissionRepository;
            _elasticsearchService = elasticsearchService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<PermissionResponse> Handle(RequestPermissionCommand command, CancellationToken cancellationToken)
        {

            // Validar que no exista otra solicitud para el mismo empleado y fecha
            var existingPermission = await _permissionRepository.GetAsync(p => p.EmployeeForename == command.EmployeeForename
                                                                          && p.EmployeSurname == command.EmployeSurname
                                                                          && p.PermissionTypeId == command.PermissionTypeId
                                                                          && p.PermissionDate == command.PermissionDate);

            if (existingPermission.Any())
            {
                throw new InvalidOperationException($"A permission request for employee '{command.EmployeeForename} {command.EmployeSurname}' on PermissionType {existingPermission?.FirstOrDefault()?.PermissionType?.Description} already exists.");
            }


            if(!(await _unitOfWork.PermissionTypeRepository.AnyAsync(command.PermissionTypeId)))
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
            var type =  await _unitOfWork.PermissionTypeRepository.GetByIdAsync(command.PermissionTypeId);
            permission.PermissionType = type;
            await _elasticsearchService.SaveSingleAsync(_mapper.Map<PermissionElasticsearch>(permission));
            var response = _mapper.Map<PermissionResponse>(permission);
            _logger.LogInformation($"Request Permission {JsonSerializer.Serialize(response)}");
            return response;
        }
    }
}
