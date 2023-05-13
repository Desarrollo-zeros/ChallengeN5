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
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        
        private readonly AppDbContext _dbContext;
        public PermissionRepository(IAppDbContext dbContext) : base(dbContext)
        {
            _dbContext = (AppDbContext)dbContext;
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _dbContext.Permissions.ToListAsync();
        }

        public async Task<List<Permission>> GetPermissionsByEmployeeNameAsync(string forename, string surname)
        {
            return await _dbContext.Permissions
                .Where(p => p.EmployeeForename == forename && p.EmployeSurname == surname)
                .ToListAsync();
        }

        public async Task<Permission?> GetPermissionByIdAsync(int id)
        {
            return await _dbContext.Permissions.FindAsync(id);
        }

        public async Task<IEnumerable<Permission>> GetAsync(Expression<Func<Permission, bool>> predicate)
        {
            return await _dbContext.Permissions.Include(x => x.PermissionType).Where(predicate).ToListAsync();
        }

        public async Task<Permission> GetByIdAsync(int id)
        {
            return await _dbContext.Permissions.Include(x => x.PermissionType).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
