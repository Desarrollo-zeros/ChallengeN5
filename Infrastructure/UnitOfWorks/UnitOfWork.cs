using Domain.Interface.Base;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Infrastructure.Repositories.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
      
        public IPermissionRepository PermissionRepository { get; set; }
        public IPermissionTypeRepository PermissionTypeRepository { get; set; }

        public UnitOfWork(IAppDbContext dbContext, IPermissionRepository permissionRepository, IPermissionTypeRepository permissionType)
        {
            _dbContext = (AppDbContext)dbContext;
            PermissionRepository = permissionRepository;
            PermissionTypeRepository = permissionType;
        }
       
        public async Task<int> CommitAsync()
        {
            // Save changes to database
            var result = await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
