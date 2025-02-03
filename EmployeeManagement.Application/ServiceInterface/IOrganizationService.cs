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
    public interface IOrganizationService
    {
        Task<Organization> CreateOrganization(OrganizationDTO model, string userId);

        Task<Organization> UpdateOrganization(OrganizationDTO model, int organizationId, string userId);

        Task<bool> DeleteOrganization(int organizationId, string userId);

        Task<Employee> AddAdmin(CreateEmployeeDTO employee, string userId, int organizationId);

        Task<bool> RemoveAdmin(int employeeId, string userId);


        Task<OrganizationDTO> GetOrganizationAsync(int organizationId);


        Task<List<Organization>> GetAllOrganizationData();

        Task<List<EmployeeDTO>> GetAdmin(int organizationId);

    }
}
