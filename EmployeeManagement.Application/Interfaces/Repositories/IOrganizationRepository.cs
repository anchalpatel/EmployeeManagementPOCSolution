using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.DTO;
using EmployeeManagement.Core.Entites;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IOrganizationRepository
    {
        Task<Organization> CreateOrganization(OrganizationDTO organizationDTO);
        Task<Organization> UpdateOrganization(OrganizationDTO organizationDTO, int organizationId);
        Task<bool> DeleteOrganization(int organizationId);

        Task<OrganizationDTO>GetOrganizationsDetials(int organizationId);
        // Task<Employee> AddAdmin(EmployeeDTO employeeDTO, int organizationId);
        Task<List<Organization>> GetAllOrganizationDetails();
       Task<bool> IsOrganizationExisits(int organizationId);
        Task<List<EmployeeDTO>> GetAdmin(int organizationId);
    }
}
