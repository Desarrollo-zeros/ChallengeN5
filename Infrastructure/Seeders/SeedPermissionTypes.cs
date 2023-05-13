using Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeders
{
    public partial class Seed
    {
        public static void SeedPermissionTypes(AppDbContext dbContext)
        {
            // Crear una lista de objetos PermissionType con los datos correspondientes
            var permissionTypes = new List<PermissionType>
            {
                new PermissionType { Description = "Vacaciones" },
                new PermissionType { Description = "Permiso médico" },
                new PermissionType { Description = "Permiso personal" },
                new PermissionType { Description = "Permiso por duelo" },
                new PermissionType { Description = "Permiso por matrimonio" }
            };
            // Iterar sobre la lista de objetos PermissionType para validar que no existen en la base de datos
            foreach (var permissionType in permissionTypes)
            {
                // Validar si existe algún PermissionType con el mismo Id o Description en la base de datos
                if (dbContext.PermissionTypes.Any(pt => pt.Id == permissionType.Id || pt.Description == permissionType.Description))
                {
                    continue; // Si existe, continuar con el siguiente objeto PermissionType
                }
                // Si no existe, agregar el objeto PermissionType al modelo
                dbContext.PermissionTypes.Add(permissionType);
            }
            // Guardar los cambios en la base de datos
            dbContext.SaveChanges();
        }
    }
}
