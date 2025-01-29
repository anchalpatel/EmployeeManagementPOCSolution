using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> AddRole(string userId, string roleName);
        Task<bool> UpdateRole(string userId, string newRole);
        Task<bool> RemoveRole(string userId, string roleName);
        Task<bool> RemoveAllRoles(string userId);
        Task<bool> IsInRole(string userId, string roleName);
    }
}
