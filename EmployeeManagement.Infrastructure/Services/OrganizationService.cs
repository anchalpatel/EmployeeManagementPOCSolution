﻿using EmployeeManagement.Application.DTO;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Services
{
    public class OrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IRoleRepository _roleRepository;

        public OrganizationService(IOrganizationRepository organizationRepository, UserManager<ApplicationUser> userManager,
                                   IUserRepository userRepository, IEmployeeRepository employeeRepository,
                                   ApplicationDbContext dbContex, IRoleRepository roleRepository)
        {
            _organizationRepository = organizationRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _dbContext = dbContex;
            _roleRepository = roleRepository;
        }
        public async Task<Organization> CreateOrganization(OrganizationDTO model, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }
              
                var organization = await _organizationRepository.CreateOrganization(model);

                return organization;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("User Not found");
            }
            
            catch (Exception ex)
            {
                throw new Exception("Error occured while creating organization" + ex);
            }
        }
        public async Task<Organization> UpdateOrganization(OrganizationDTO model, int organizationId, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }
               
                var organization = await _organizationRepository.UpdateOrganization(model, organizationId);

                return organization;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while Updating organization" + ex);
            }
        }
        public async Task<bool> DeleteOrganization(int organizationId, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }
                
                var isDeleted = await _organizationRepository.DeleteOrganization(organizationId);
               
                if (!isDeleted)
                {
                    throw new Exception("Error occured in deleting employee");
                }
                return isDeleted;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while Deleting organization" + ex);
            }
        }

        public async Task<Employee> AddAdmin(EmployeeDTO employee, string userId, int organizationId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                RegisterUser userdto = new RegisterUser()
                {
                    UserName = employee.FirstName,
                    Email = employee.Email,
                    Password = employee.Password
                };

                var register = await _userRepository.RegisterUserAsync(userdto);
                if(register == null)
                {
                    throw new Exception("User cannot be added");
                }

                employee.userId = register.UserId;
                var emp = await _employeeRepository.CreateEmoloyee(employee, organizationId, userId);
                if(emp == null)
                {
                    throw new Exception("Employee cannot be added");
                }

                //var adminUser = await _userManager.FindByEmailAsync(employee.Email);
                //Adding superadmin role
                var result = await _roleRepository.AddRole(emp.UserId, "Admin");

                if(result)
                {
                    return emp;
                }
                else
                {
                    throw new Exception("Admin cannot be added");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while Deleting organization" + ex);
            }
        }
        public async Task<bool> RemoveAdmin(int employeeId, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                var emp = await _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == employeeId);
                var empUserId = emp.UserId;

                var isEmpDeleted = await _employeeRepository.DeleteEmployee(employeeId);
                if (!isEmpDeleted)
                {
                    throw new Exception("Employe can not be deleted");
                }
                var removeEmpUser = await _userRepository.RemoveUserAsync(empUserId);
                if (!removeEmpUser)
                {
                    throw new Exception("User can not be deleted");
                }
                return true;
                
            }catch(Exception ex)
            {
                throw new Exception("Admin can not be removed : " + ex.Message);
            }
           
        }

        public async Task<OrganizationDTO> GetOrganizationAsync(int organizationId)
        {
            try
            {
                var organization = await _organizationRepository.GetOrganizationsDetials(organizationId);
                if(organization == null)
                {
                    throw new Exception("Organization not found");
                }
                return organization;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Organization>> GetAllOrganizationData()
        {
            try
            {
                var organization = await _organizationRepository.GetAllOrganizationDetails();
                if (organization == null)
                {
                    throw new Exception("Organization not found");
                }
               

                return organization;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<EmployeeDTO>> GetAdmin(int organizationId)
        {
            try
            {
                var admin = await _organizationRepository.GetAdmin(organizationId);
                if (admin == null)
                {
                    throw new Exception("Admin not found");
                }

                return admin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
