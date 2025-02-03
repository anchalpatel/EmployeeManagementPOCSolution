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
    public interface IEmployeeService
    {
        Task<Employee> CreateEmployee(CreateEmployeeDTO model, string userId, int organizationId);

        Task<bool> RemoveEmpoyee(int employeeId, string userId);

        Task<Employee> UpdateEmployee(UpdateEmployeeDTO model, int employeeId, string userId);

        Task<IEnumerable<EmployeeDTO>> GetAllEmployees(int organizationId);

        Task<EmployeeDTO> GetEmployeeDetail(int emploeeId);

        Task<IEnumerable<Employee>> GetEmployeeCreatedByUser(string userId, int organizationId);


        Task<IEnumerable<EmployeeDTO>> GetHrList(int organizationId, string userId);

        Task<IEnumerable<EmployeeDTO>> GetEmployeeInUserRole(int organizationId, string userId);

    }
}
