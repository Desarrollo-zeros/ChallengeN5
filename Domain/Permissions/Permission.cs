using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Permissions
{
    [Table("permissions")]
    public class Permission : Entity<int>
    {
        
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string EmployeeForename { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string EmployeSurname { get; set; }

        [ForeignKey("PermissionType")]
        public int PermissionTypeId { get; set; }

        public DateTime PermissionDate { get; set; }

        public virtual PermissionType PermissionType { get; set; }
    }

}
