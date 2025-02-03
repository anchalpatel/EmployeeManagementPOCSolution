using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Core.Entites;

namespace EmployeeManagement.Infrastructure.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> CreateEmoloyee(CreateEmployeeDTO employee, int organiationId, string ceratedBy);
        Task<bool> DeleteEmployee(int employeeId);
        Task<EmployeeDTO> GetEmployeeDetails(int employeeId);
        Task<Employee> GetEmployeeByUserId(string userId);
        Task<Employee> UpdateEmloyee(UpdateEmployeeDTO employee, int employeeId, string createdBy, string reqRole);
        Task<IEnumerable<EmployeeDTO>> GetEmployees(int organizationId);
        Task<IEnumerable<Employee>> GetEmployeesCreatedByUser(string userId, int organizationId);
        Task<IEnumerable<EmployeeDTO>> GetAllHr(int organizationId);
        Task<IEnumerable<EmployeeDTO>> GetEmployeesInuserRole(int organizationId);
    }
}
