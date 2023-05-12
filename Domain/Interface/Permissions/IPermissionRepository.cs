using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Permissions;

namespace Domain.Interface.Permissions
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<List<Permission>> GetPermissionsByEmployeeNameAsync(string forename, string surname);
        Task<Permission?> GetPermissionByIdAsync(int id);


    }
}
