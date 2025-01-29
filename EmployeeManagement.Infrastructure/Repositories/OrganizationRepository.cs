using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.DTO;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrganizationRepository(ApplicationDbContext context, IRoleRepository roleRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        //public async Task<Employee> AddAdmin(EmployeeDTO employeeDTO, int organizationId)
        //{
        //    var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId);
        //    if(organization == null)
        //    {
        //        throw new ArgumentException("Organization not found");
        //    }


        //}

        public async Task<Organization> CreateOrganization(OrganizationDTO organizationDTO)
        {
            var organization = new Organization()
            {
                Name = organizationDTO.Name,
                Address = organizationDTO.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            var result = await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();
            return organization;
        }

        public async Task<bool> DeleteOrganization(int organizationId)
        {
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == organizationId && o.IsDeleted == false);

            if (organization == null)
            {
                throw new ArgumentException("Organization does not exist or has already been deleted");
            }

            organization.IsDeleted = true;
            var employees = await _context.Employees
                .Where(e => e.OrganizationId == organization.Id && e.IsDeleted == false)
                .ToListAsync();

            foreach (var employee in employees)
            {
                employee.IsDeleted = true;

                var removeUserFromAllRoles = await _roleRepository.RemoveAllRoles(employee.UserId);
                if (!removeUserFromAllRoles)
                {
                    throw new Exception($"Employee {employee.FirstName} {employee.LastName} could not be removed from roles");
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<OrganizationDTO> GetOrganizationsDetials(int organizationId)
        {
            Organization organizationDetails = await _context.Organizations.Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == organizationId && o.IsDeleted == false);
            List<EmployeeDTO> employees = new List<EmployeeDTO>();
            foreach (var emp in organizationDetails.Employees)
            {
                EmployeeDTO employee = new EmployeeDTO()
                {
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    Email = emp.Email,
                    Id = emp.Id,
                    CreatedBy = emp.CreatedBy
                };
                employees.Add(employee);
            }
            OrganizationDTO organization = new OrganizationDTO()
            {
                Name = organizationDetails.Name,
                Address = organizationDetails.Address,
                CreatedAt = organizationDetails.CreatedAt,
                Id = organizationDetails.Id,
                Employees = employees
            };
            return organization;
        }

        public async Task<List<Organization>> GetAllOrganizationDetails()
        {
            List<Organization> organizationDetails = await _context.Organizations.Include(o => o.Employees).Where(o => o.IsDeleted == false).ToListAsync();
            var organizationList = new List<Organization>();
            foreach (var org in organizationDetails)
            {
                var employees = new List<Employee>();
                foreach(var emp in org.Employees)
                {
                    var newEmployee = new Employee()
                    {
                        Id = emp.Id,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        Email = emp.Email,
                        Address = emp.Address,
                        CreatedAt = emp.CreatedAt,
                        PhoneNumber = emp.PhoneNumber,
                        OrganizationId = emp.OrganizationId,
                        UserId = emp.UserId,
                        CreatedBy = emp.CreatedBy
                    };
                    employees.Add(newEmployee);
                }
                Organization newOrganization = new Organization()
                {
                    Id = org.Id,
                    Name = org.Name,
                    Address = org.Address,
                    CreatedAt = org.CreatedAt,
                    Employees = employees
                };
                organizationList.Add(newOrganization);
            }
                return organizationList;
        }

        public async Task<bool> IsOrganizationExisits(int organizationId)
        {
            return await _context.Organizations.AnyAsync(o => o.Id == organizationId);
        }

        public async Task<Organization> UpdateOrganization(OrganizationDTO organizationDTO, int organizationId)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId && o.IsDeleted == false);

            if (organization == null)
            {
                throw new ArgumentException("Organization Do not exists");
            }

            else
            {
                organization.Name = organizationDTO.Name;
                organization.Address = organizationDTO.Address;
                organization.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();  
                return organization;
            }
        }

        public async Task<List<EmployeeDTO>> GetAdmin(int organizationId)
        {
            var employeeList = await _context.Employees.Where(e => e.OrganizationId == organizationId && e.IsDeleted == false).ToListAsync();
            List<EmployeeDTO> adminList = new List<EmployeeDTO>();
            foreach (var emp in employeeList)
            {
                var user = await _userManager.FindByIdAsync(emp.UserId);
                if(user == null)
                {
                    throw new Exception("User not founnd");
                }
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    EmployeeDTO admin = new EmployeeDTO() {
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        PhoneNumber = emp.PhoneNumber,
                        userId = emp.UserId,
                        Address = emp.Address,
                        Email = emp.Email,
                        CreareAt = emp.CreatedAt,
                        CreatedBy = emp.CreatedBy,
                        Id = emp.Id
                    };
                    adminList.Add(admin);
                   
                }
            }
            return adminList;
        }
    }
}
