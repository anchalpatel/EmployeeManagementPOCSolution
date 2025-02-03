using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Core.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.ServiceInterface
{
    public interface IAdminService
    {
        Task<Employee> CreateHr(CreateEmployeeDTO employee, string userId, int organizationId);

        Task<bool> RemoveHr(int employeeId, string userId);

    }
}
