using Domain.Interface.Base;
using Domain.Interface.Permissions;
using Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Permissions
{
    public class PermissionTypeRepository : Repository<PermissionType>, IPermissionTypeRepository
    {
        
        private readonly AppDbContext _dbContext;
        public PermissionTypeRepository(IAppDbContext dbContext) : base(dbContext)
        {
            _dbContext = (AppDbContext)dbContext;
        }

        public async Task<PermissionType?> GetByDescription(string description)
        {
            return await _dbContext.PermissionTypes.FirstOrDefaultAsync(x => x.Description == description);
        }

        public async Task<IEnumerable<PermissionType>> GetAsync(Expression<Func<PermissionType, bool>> predicate)
        {
            return await _dbContext.PermissionTypes.Where(predicate).ToListAsync();
        }
    }
}
