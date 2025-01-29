using System.Data;
using System.Net;
using EmployeeManagement.Application.DTO;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleRepository _roleRepository;

        public EmployeeRepository(ApplicationDbContext dbContext, RoleManager<ApplicationRole> roleManager
            ,IUserRepository userRepository, UserManager<ApplicationUser> userManager, IRoleRepository roleRepository)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
        }
        public async Task<Employee> CreateEmoloyee(EmployeeDTO model, int organizationId, string createdBy)
        {
            var emp = await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == model.Email && u.IsDeleted == true);
            if(emp!= null)
            {
                emp.IsDeleted = false;
                emp.FirstName = model.FirstName;
                emp.LastName = model.LastName;
                emp.Email = model.Email;
                emp.OrganizationId = organizationId;
                emp.Address = model.Address;
                emp.UserId = model.userId;
                emp.PhoneNumber = model.PhoneNumber;
                emp.CreatedBy = createdBy;
                emp.CreatedAt = DateTime.UtcNow;
                emp.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                return emp;
            }
            var employee = new Employee()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                OrganizationId = organizationId,
                Address = model.Address,
                UserId = model.userId,
                PhoneNumber = model.PhoneNumber,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newEmployee = await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployee(int employeeId)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && e.IsDeleted == false);
            if (employee == null)
            {
                throw new ArgumentException("Employee with id " + employeeId + " not found"); 
            }
            employee.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllHr(int organizationId)
        {
            var emp = await _dbContext.Employees.Where(e => e.OrganizationId == organizationId && e.IsDeleted == false).ToListAsync();
            if (emp == null) {
                return null;
            }
            List<EmployeeDTO> result = new List<EmployeeDTO>();
            foreach(var employee in emp)
            {
                bool isHR = await _roleRepository.IsInRole(employee.UserId, "HR");
                if(isHR)
                {
                    EmployeeDTO newEmp = new EmployeeDTO()
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber,
                        Address = employee.Address,
                        userId = employee.UserId,
                        CreatedBy = employee.CreatedBy,
                        CreareAt = employee.CreatedAt,
                    };
                    result.Add(newEmp);
                }
            }
            return result;
        }

        public async Task<Employee> GetEmployeeByUserId(string userId)
        {
            var emp = await _dbContext.Employees.FirstOrDefaultAsync(e => e.UserId == userId && e.IsDeleted == false);
            return emp;
        }

        public async Task<EmployeeDTO> GetEmployeeDetails(int employeeId)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == employeeId && emp.IsDeleted == false);
            if(employee == null)
            {
                return null;
            }
            EmployeeDTO model = new EmployeeDTO()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                userId = employee.UserId,
                CreatedBy = employee.CreatedBy,
                CreareAt = employee.CreatedAt,
            };
            var user = await _userManager.FindByIdAsync(employee.UserId);
            var roles = await _userManager.GetRolesAsync(user);
            model.Roles = (List<string>?)roles;
            return model ;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetEmployees(int organizationId)
        {
            var employees = await _dbContext.Employees.Where(e => e.OrganizationId == organizationId && e.IsDeleted == false).ToListAsync();
            List<EmployeeDTO> empList = new List<EmployeeDTO>();
            foreach (var employee in employees)
            {
                var user = await _userManager.FindByIdAsync(employee.UserId);
                if(user == null)
                {
                    throw new Exception("Employee is not registered");
                }
                var userRoles = await _userManager.GetRolesAsync(user);

                var roleIds = await _dbContext.UserRoles
                                              .Where(ur => ur.UserId == user.Id && ur.IsDeleted == false)
                                              .Select(ur => ur.RoleId)
                                              .ToListAsync();

                var validRoleNames = await _dbContext.Roles
                                                      .Where(r => roleIds.Contains(r.Id))
                                                      .Select(r => r.Name) 
                                                      .ToListAsync();

                var validRoles = userRoles.Where(role => validRoleNames.Contains(role)).ToList();

                EmployeeDTO emp = new EmployeeDTO()
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Address = employee.Address,
                    userId = employee.UserId,
                    CreatedBy = employee.CreatedBy,
                    CreareAt = employee.CreatedAt,
                    Roles = validRoles
                };
                empList.Add(emp);
            }
            return empList;
        }

        public  async Task<IEnumerable<Employee>> GetEmployeesCreatedByUser(string userId, int organizationId)
        {
            IEnumerable<Employee> employees = await _dbContext.Employees.Where(e => e.CreatedBy == userId && e.OrganizationId == organizationId && e.IsDeleted == false).ToListAsync();
            return employees;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesInuserRole(int organizationId)
        {
            var employees = await _dbContext.Employees.Where(e => e.OrganizationId == organizationId && e.IsDeleted == false).ToListAsync();
            List<EmployeeDTO> empList = new List<EmployeeDTO>();
            foreach (var emp in employees)
            {
                var empUser = await _userManager.FindByIdAsync(emp.UserId);
                if (empUser == null)
                {
                    throw new Exception("User corresponds to employe not found");
                }
                var roles = await _userManager.GetRolesAsync(empUser);
                var roleIds = await _dbContext.UserRoles.Where(ur => ur.UserId == emp.UserId && ur.IsDeleted == false)
                                                           .Select(ur => ur.RoleId)
                                                           .ToListAsync();
                var validRole = await _dbContext.Roles
                                                      .Where(r => roleIds.Contains(r.Id) && r.Name == "User")
                                                      .ToListAsync();
               
                if(validRole != null)
                {
                    EmployeeDTO employee = new EmployeeDTO()
                    {
                        Id = emp.Id,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        Email = emp.Email,
                        PhoneNumber = emp.PhoneNumber,
                        Address = emp.Address,
                        userId = emp.UserId,
                        CreatedBy = emp.CreatedBy,
                        CreareAt = emp.CreatedAt,
                    };
                    empList.Add(employee);
                }
            }
            return empList;
        }

        public async Task<Employee> UpdateEmloyee(UpdateEmployeeDTO employeeDto, int employeeId, string createdBy, string reqRole)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Id  == employeeId && emp.IsDeleted == false);
            if(employee == null)
            {
                return null;
            }
            var empUser = await _userManager.FindByIdAsync(employee.UserId);
            var empRole = await _userManager.GetRolesAsync(empUser);

            var roleIds = await _dbContext.UserRoles
                                            .Where(ur => ur.UserId == empUser.Id && ur.IsDeleted == false)
                                            .Select(ur => ur.RoleId)
                                            .ToListAsync();

            var validRoleNames = await _dbContext.Roles
                                                  .Where(r => roleIds.Contains(r.Id))
                                                  .Select(r => r.Name)
                                                  .ToListAsync();

            var validRoles = empRole.Where(role => validRoleNames.Contains(role)).ToList();
            if (validRoles.Contains("Admin") && reqRole != "SuperAdmin")
            {
                throw new Exception("Only Super admin can edit admin");
            }
            if (validRoles.Contains("HR") && reqRole != "Admin")
            {
                throw new Exception("Only admin can edit HR");
            }

            if (validRoles.Contains("User") && (!(reqRole == "Admin") && (!(reqRole == "HR" && employee.CreatedBy == createdBy))))
            { 
                throw new Exception("You cannot update an employee that you have not created."); 
            }

            if (employeeDto.FirstName != null && employeeDto.FirstName != employee.FirstName) employee.FirstName = employeeDto.FirstName;
            if (employeeDto.LastName != null && employeeDto.LastName != employee.LastName)  employee.LastName = employeeDto.LastName;
            if (employeeDto.Email != null && employeeDto.Email != employee.Email)
            {
                bool userUpdate = await _userRepository.UpdateEmail(employee.UserId, employeeDto.Email);
                if (!userUpdate)
                {
                    throw new Exception("Email can not be updated");
                }
                employee.Email = employeeDto.Email;
            }
            if (employeeDto.Address != null && employeeDto.Address != employee.Address) employee.Address = employeeDto.Address;
            if (employeeDto.PhoneNumber != null && employeeDto.PhoneNumber != employee.PhoneNumber) employee.PhoneNumber = employeeDto.PhoneNumber;
            if(employeeDto.Roles != null && !validRoles.Contains(employeeDto.Roles))
            {
                if (reqRole != "Admin")
                {
                    throw new Exception("Only Admins can change employee roles.");
                }
                bool updateEmp = await _roleRepository.UpdateRole(employee.UserId, employeeDto.Roles);
                if (!updateEmp)
                {
                    throw new Exception("Employee role cannot be updated");
                }
                
            }
            employee.UpdatedAt = DateTime.UtcNow;
             await _dbContext.SaveChangesAsync();
            return employee;
        }
    }
}
