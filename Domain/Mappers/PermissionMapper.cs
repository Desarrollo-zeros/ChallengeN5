using AutoMapper;
using Domain.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Mappers
{
    public class PermissionMapper : Profile
    {
        public PermissionMapper()
        {
            CreateMap<Permission, PermissionElasticsearch>()
                .ForMember(d => d.PermissionType, opts => opts.MapFrom(s => s.PermissionType.Description))
                .ReverseMap();
            CreateMap<PermissionElasticsearch, PermissionResponse>().ReverseMap();
            CreateMap<Permission, PermissionResponse>()
                .ForMember(d => d.PermissionType, opts => opts.MapFrom(s => s.PermissionType.Description))
                .ReverseMap();
        }
    }
}
