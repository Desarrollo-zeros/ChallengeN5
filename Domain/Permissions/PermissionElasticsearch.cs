using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;
using Domain.Interface.Base;
using Nest;

namespace Domain.Permissions
{
    [ElasticsearchType(Name = "permission")]
    public class PermissionElasticsearch : Entity, IEntity
    {
        
        [Text(Name = "employee_forename")]
        public string EmployeeForename { get; set; }

        [Text(Name = "employee_surname")]
        public string EmployeSurname { get; set; }

        [Number(Name = "permission_type")]
        public string PermissionType { get; set; }

        [Date(Name = "permission_date")]
        public DateTime PermissionDate { get; set; }
    }
}
