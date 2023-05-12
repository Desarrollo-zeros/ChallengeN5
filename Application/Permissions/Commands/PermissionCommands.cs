using Domain.Permissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions.Commands
{
    // Comandos
    public record RequestPermissionCommand : IRequest<Permission>
    {
        

        public string EmployeeForename { get; set; }
        public string EmployeSurname { get; set; }
        public int PermissionTypeId { get; set; }
        public DateTime PermissionDate { get; set; }
    }
    public record ModifyPermissionCommand : IRequest<Permission>
    {
        
        public int Id { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeSurname { get; set; }
        public int PermissionTypeId { get; set; }
        public DateTime PermissionDate { get; set; }
    }

}
