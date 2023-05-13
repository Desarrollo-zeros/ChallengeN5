using Application.Permissions.Commands;
using AutoMapper;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Permissions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Permissions.Querys
{
    public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionResponse>>, IGetPermissionsQueryHandler
    {
       
        private readonly IElasticSearchApplication<PermissionElasticsearch> _elasticSearchApplication;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPermissionsQueryHandler> _logger;
        public GetPermissionsQueryHandler( 
            IElasticSearchApplication<PermissionElasticsearch> elasticSearchApplication,
            IMapper mapper,
            ILogger<GetPermissionsQueryHandler> logger)
        {
            _elasticSearchApplication = elasticSearchApplication;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PermissionResponse>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var all = await _elasticSearchApplication.GetAll();
            _logger.LogInformation($"Get Permission {JsonSerializer.Serialize(all)}");
            return _mapper.Map<List<PermissionResponse>>(all);
        }
    }
}
