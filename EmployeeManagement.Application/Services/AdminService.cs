using EmployeeManagement.Application.ServiceInterface;
using EmployeeManagement.Core.DTO;

using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEmployeeRepository _emploeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRoleRepository _roleRepository;

        public AdminService(IEmployeeRepository emploeeRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager, 
                                    ApplicationDbContext dbContext, IEmployeeRepository employeeRepository,
                                    IRoleRepository roleRepository)
        {
            _emploeeRepository = emploeeRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _dbContext = dbContext;
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }

        public async Task<Employee> CreateHr(CreateEmployeeDTO employee, string userId, int organizationId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                RegisterUser userdto = new RegisterUser()
                {
                    UserName = employee.FirstName,
                    Email = employee.Email,
                    Password = employee.Password
                };

                var register = await _userRepository.RegisterUserAsync(userdto);
               
                employee.userId = register.UserId;
                var emp = await _emploeeRepository.CreateEmoloyee(employee, organizationId, userId);
                //if (emp == null)
                //{
                //    throw new Exception("Employee cannot be added");
                //}

                // var adminUser = await _userManager.FindByEmailAsync(employee.Email);

                var result = await _roleRepository.AddRole(emp.UserId, "HR");

                if (result)
                {
                    return emp;
                }
                else
                {
                    throw new Exception("HR cannot be added");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while Creating HR" + ex);
            }
        }

        public async Task<bool> RemoveHr(int employeeId, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                var emp = await _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == employeeId);
              
                if (!await _employeeRepository.DeleteEmployee(employeeId))
                {
                    throw new Exception("Employe can not be deleted");
                }
                
                if (!await _userRepository.RemoveUserAsync(emp.UserId))
                {
                    throw new Exception("User can not be deleted");
                }
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Admin can not be removed : " + ex.Message);
            }

        }

    }
}

