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
    [Table("permissionType")]
    public class PermissionType : Entity<int>
    {
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Description { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
