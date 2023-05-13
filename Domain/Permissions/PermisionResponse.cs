using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Permissions
{
    public class PermissionResponse
    {
        public int Id { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeSurname { get; set; }
        public string PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }
    }
}
