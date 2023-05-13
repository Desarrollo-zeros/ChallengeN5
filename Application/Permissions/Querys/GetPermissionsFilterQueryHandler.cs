using AutoMapper;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Permissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions.Querys
{
    public class GetPermissionsFilterQueryHandler : IRequestHandler<GetPermissionsFilterQuery, PermissionResponse>, IGetPermissionsFilterQueryHandler
    {
        
        private readonly IMapper _mapper;
        private readonly IElasticSearchApplication<PermissionElasticsearch> _elasticsearchApplication;

        public GetPermissionsFilterQueryHandler(
             IElasticSearchApplication<PermissionElasticsearch> elasticsearchApplication,
             IMapper mapper)
        {
            _mapper = mapper;
            _elasticsearchApplication = elasticsearchApplication;
        }

        public async Task<PermissionResponse> Handle(GetPermissionsFilterQuery request, CancellationToken cancellationToken)
        {
           var response = await _elasticsearchApplication.GetAsync(request.id);
            return _mapper.Map<PermissionResponse>(response);
        }
    }
}
