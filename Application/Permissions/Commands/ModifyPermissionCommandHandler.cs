
using AutoMapper;
using Domain.Exceptions;
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
    public class ModifyPermissionCommandHandler : IRequestHandler<ModifyPermissionCommand, PermissionResponse>, IModifyPermissionCommandHandler
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IElasticSearchApplication<PermissionElasticsearch> _elasticsearchService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ModifyPermissionCommandHandler> _logger;
        public ModifyPermissionCommandHandler(IPermissionRepository permissionRepository,
            IElasticSearchApplication<PermissionElasticsearch> elasticsearchService, 
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ModifyPermissionCommandHandler> logger)
        {
            _permissionRepository = permissionRepository;
            _elasticsearchService = elasticsearchService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PermissionResponse> Handle(ModifyPermissionCommand command, CancellationToken cancellationToken)
        {

            var permission = await _permissionRepository.GetByIdAsync(command.Id);

            if (permission == null)
                throw new NotFoundException(nameof(Permission), command.Id);
            if (!(await _unitOfWork.PermissionTypeRepository.AnyAsync(command.PermissionTypeId)))
            {
                throw new InvalidOperationException($"A permission Type request {command.PermissionTypeId} no exist");
            }

            permission.EmployeeForename = command.EmployeeForename;
            permission.EmployeSurname = command.EmployeSurname;
            permission.PermissionTypeId = command.PermissionTypeId;
            permission.PermissionDate = command.PermissionDate;

            _permissionRepository.Update(permission);
            await _unitOfWork.CommitAsync();
            await _elasticsearchService.SaveSingleAsync(_mapper.Map<PermissionElasticsearch>(permission));
            var response = _mapper.Map<PermissionResponse>(permission);
            _logger.LogInformation($"Modify Permission {JsonSerializer.Serialize(response)}");
            return _mapper.Map<PermissionResponse>(permission); ;
        }
    }
}
