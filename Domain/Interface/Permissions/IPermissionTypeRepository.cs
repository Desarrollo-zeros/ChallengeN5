using Domain.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Permissions
{
    public interface IPermissionTypeRepository : IRepository<PermissionType>
    {
        Task<PermissionType?> GetByDescription(string description);
    }
}
