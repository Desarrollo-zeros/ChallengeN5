using Domain.Interface.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.UnitOfWorks
{
    public interface IUnitOfWork
    {
        public IPermissionRepository PermissionRepository { get;  }
        public IPermissionTypeRepository PermissionTypeRepository { get;  }
        Task<int> CommitAsync();
    }
}
