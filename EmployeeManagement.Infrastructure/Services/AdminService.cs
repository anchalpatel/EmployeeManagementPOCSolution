using EmployeeManagement.Application.DTO;
using EmployeeManagement.Application.Interfaces.Repositories;

using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Services
{
    public class AdminService
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

        public async Task<Employee> CreateHr(EmployeeDTO employee, string userId, int organizationId)
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
                if (register == null)
                {
                    throw new Exception("User cannot be added");
                }

                employee.userId = register.UserId;
                var emp = await _emploeeRepository.CreateEmoloyee(employee, organizationId, userId);
                if (emp == null)
                {
                    throw new Exception("Employee cannot be added");
                }

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

            }
            catch (Exception ex)
            {
                throw new Exception("Admin can not be removed : " + ex.Message);
            }

        }

    }
}

