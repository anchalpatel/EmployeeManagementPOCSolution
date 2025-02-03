using System.Data;
using System.Net;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;
using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleRepository _roleRepository;

        public EmployeeRepository(ApplicationDbContext dbContext,
                                  IUserRepository userRepository,
                                  UserManager<ApplicationUser> userManager,
                                  IRoleRepository roleRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
        }
        public async Task<Employee> CreateEmoloyee(CreateEmployeeDTO model, int organizationId, string createdBy)
        {
            try
            {
                var emp = await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == model.Email && u.IsDeleted == true);
                if (emp != null)
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
                if(newEmployee == null)
                {
                    throw new Exception("Employee cannot be created");
                }
                await _dbContext.SaveChangesAsync();
                return employee;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while creating employee " + ex);
            }
        }

        public async Task<bool> DeleteEmployee(int employeeId)
        {
            try
            {
                var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && e.IsDeleted == false);
                if (employee == null)
                {
                    throw new ArgumentException("Employee with id " + employeeId + " not found");
                }
                employee.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                throw new Exception("Error ocured while deleting employee " + ex);
            }
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllHr(int organizationId)
        {
            try
            {
                return await (from emp in _dbContext.Employees
                              join userRole in _dbContext.UserRoles
                              on emp.UserId equals userRole.UserId
                              join role in _dbContext.Roles
                              on userRole.RoleId equals role.Id
                              where emp.OrganizationId == organizationId
                              && emp.IsDeleted == false
                              && role.IsDeleted == false
                              && userRole.IsDeleted == false
                              && role.Name == "HR"
                              select new EmployeeDTO
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
                              }).ToListAsync();


            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while fetching hr data " + ex);
            }

        }

        public async Task<Employee> GetEmployeeByUserId(string userId)
        {
            try
            {
                var emp = await _dbContext.Employees.FirstOrDefaultAsync(e => e.UserId == userId && e.IsDeleted == false);
                if (emp == null && !(await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId),"SuperAdmin")))
                {
                    throw new Exception("Employee not found");
                }
                return emp;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while getting employee data " + ex);
            }
        }

        public async Task<EmployeeDTO> GetEmployeeDetails(int employeeId)
        {
            try
            {
                var employee = await (from emp in _dbContext.Employees
                                      join userRole in _dbContext.UserRoles
                                      on emp.UserId equals userRole.UserId
                                      join role in _dbContext.Roles
                                      on userRole.RoleId equals role.Id
                                      where emp.Id == employeeId
                                      && emp.IsDeleted == false
                                      && role.IsDeleted == false
                                      && userRole.IsDeleted == false
                                      group role.Name by new
                                      {
                                          emp.Id,
                                          emp.FirstName,
                                          emp.LastName,
                                          emp.Email,
                                          emp.UserId,
                                          emp.OrganizationId,
                                          emp.Address,
                                          emp.CreatedAt,
                                          emp.CreatedBy,
                                          emp.PhoneNumber
                                      } into empRoles
                                      select new EmployeeDTO
                                      {
                                          Id = empRoles.Key.Id,
                                          FirstName = empRoles.Key.FirstName,
                                          LastName = empRoles.Key.LastName,
                                          Email = empRoles.Key.Email,
                                          PhoneNumber = empRoles.Key.PhoneNumber,
                                          Address = empRoles.Key.Address,
                                          userId = empRoles.Key.UserId,
                                          CreatedBy = empRoles.Key.CreatedBy,
                                          CreareAt = empRoles.Key.CreatedAt,
                                          Roles = empRoles.ToList()
                                      }).FirstOrDefaultAsync();
                if (employee == null)
                {
                    throw new Exception("Employee Not Found");
                }
                return employee;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while fetching emplpoyee details " + ex);
            }
        }

        public async Task<IEnumerable<EmployeeDTO>> GetEmployees(int organizationId)
        {
            try
            {
                var empList = await (from emp in _dbContext.Employees
                                     join userRoles in _dbContext.UserRoles
                                     on emp.UserId equals userRoles.UserId
                                     join role in _dbContext.Roles
                                     on userRoles.RoleId equals role.Id
                                     where emp.IsDeleted == false
                                     && role.IsDeleted == false
                                     && userRoles.IsDeleted == false
                                     group role by new
                                     {
                                         emp.Id,
                                         emp.FirstName,
                                         emp.LastName,
                                         emp.Email,
                                         emp.PhoneNumber,
                                         emp.Address,
                                         emp.UserId,
                                         emp.CreatedBy,
                                         emp.CreatedAt
                                     } into g
                                     select new EmployeeDTO
                                     {
                                         Id = g.Key.Id,
                                         FirstName = g.Key.FirstName,
                                         LastName = g.Key.LastName,
                                         Email = g.Key.Email,
                                         PhoneNumber = g.Key.PhoneNumber,
                                         Address = g.Key.Address,
                                         userId = g.Key.UserId,
                                         CreatedBy = g.Key.CreatedBy,
                                         CreareAt = g.Key.CreatedAt,
                                         Roles = g.Select(role => role.Name).ToList()
                                     }).ToListAsync();


                
                return empList;
            }
            catch(Exception ex)
            {
                throw new Exception("Exception occured while fetching employee data " + ex.Message);
            }
        }

        public  async Task<IEnumerable<Employee>> GetEmployeesCreatedByUser(string userId, int organizationId)
        {
            try
            {
                IEnumerable<Employee> employees = await _dbContext.Employees.Where(e => e.CreatedBy == userId && e.OrganizationId == organizationId && e.IsDeleted == false).ToListAsync();
                return employees;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while fetching employee details created by user " + ex);
            }
        }

        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesInuserRole(int organizationId)
        {
            try
            {
                return await (from emp in _dbContext.Employees
                              join userRole in _dbContext.UserRoles
                              on emp.UserId equals userRole.UserId
                              join role in _dbContext.Roles
                              on userRole.RoleId equals role.Id
                              where emp.OrganizationId == organizationId
                              && emp.IsDeleted == false
                              && role.IsDeleted == false
                              && userRole.IsDeleted == false
                              && role.Name == "User"
                              select new EmployeeDTO
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
                              }).ToListAsync();


            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while fetching user role employees " + ex);
            }
        }

        public async Task<Employee> UpdateEmloyee(UpdateEmployeeDTO employeeDto, int employeeId, string createdBy, string reqRole)
        {
            try
            {
                var employee = await _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == employeeId && emp.IsDeleted == false);
                if (employee == null)
                {
                    throw new Exception("Requested employee not found");
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
                if (employeeDto.LastName != null && employeeDto.LastName != employee.LastName) employee.LastName = employeeDto.LastName;
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
                if (employeeDto.Roles != null && !validRoles.Contains(employeeDto.Roles))
                {
                    if (reqRole != "Admin")
                    {
                        throw new Exception("Only Admins can change employee roles.");
                    }

                    if (!await _roleRepository.UpdateRole(employee.UserId, employeeDto.Roles))
                    {
                        throw new Exception("Employee role cannot be updated");
                    }

                }
                employee.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                return employee;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while updating employee " + ex);
            }
        }
    }
}
