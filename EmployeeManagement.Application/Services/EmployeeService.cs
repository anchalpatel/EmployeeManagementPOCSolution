using System.Collections;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;
using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Application.ServiceInterface;

namespace EmployeeManagement.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleRepository _roleRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrganizationRepository _organizationRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager, IRoleRepository roleRepository, ApplicationDbContext dbContext, IOrganizationRepository organizationRepository)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
            _dbContext = dbContext;
            _organizationRepository = organizationRepository;
        }
        public async Task<Employee> CreateEmployee(CreateEmployeeDTO model, string userId, int organizationId)
        {
            try
            {
                if(userId==null || organizationId == 0 || (model.FirstName == null || model.LastName == null || model.Email == null || model.PhoneNumber == null || model.Password == null))
                {
                    throw new Exception("UserId and Organizaion id is required");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if(user == null) throw new ArgumentException("User not found");

                
                var newUser = await _userRepository.RegisterUserAsync(new RegisterUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Password = model.Password
                });
                //if(newUser == null)
                //{
                //    throw new Exception("Error occured during creating new user");
                //}
                model.userId = newUser.UserId;
                var employee = await _employeeRepository.CreateEmoloyee(model, organizationId, userId);
                //if(employee == null)
                //{
                //    throw new Exception("Employee can not be created");
                //}
                if (!await _roleRepository.AddRole(newUser.UserId, "User"))
                {
                    throw new Exception("User can not be added in the role");
                }
                return employee;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured while creating employee" + ex.Message);
            }
        }
        public async Task<bool> RemoveEmpoyee(int employeeId, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new UnauthorizedAccessException("User not found");

                var emp = await _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == employeeId && emp.IsDeleted == false);
                var empUserId = emp.UserId;
                if(emp.CreatedBy != userId) throw new UnauthorizedAccessException("You can delete only those employee whome you have created");

                if (!await _employeeRepository.DeleteEmployee(employeeId)) throw new Exception("Employe can not be deleted");

                if (!await _userRepository.RemoveUserAsync(empUserId)) throw new Exception("User can not be deleted");

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Admin can not be removed : " + ex.Message);
            }

        }
        public async Task<Employee> UpdateEmployee(UpdateEmployeeDTO model, int employeeId, string userId)
        {
            try
            {
                if(employeeId == null || userId == null) throw new ArgumentException("Organization or userId not provided");

                var user = await _userManager.FindByIdAsync(userId);
                if(user == null) throw new Exception($"User with is {user.Id} not found");

                var roles = await _userManager.GetRolesAsync(user);
                
                var updatedEmployee = await _employeeRepository.UpdateEmloyee(model, employeeId, userId, roles[0]);
                //if(updatedEmployee == null)
                //{
                //    throw new Exception("User can not be updated");
                //}
                
                return updatedEmployee;
            }
            catch(Exception ex)
            {
                throw new Exception("Exceptionoccured while updating employee " + ex.Message);
            }
           
        }
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployees(int organizationId)
        {
            try
            {
                if (!(await _organizationRepository.IsOrganizationExisits(organizationId)))
                {
                    throw new Exception("Oranization do not exists");
                }
                return await _employeeRepository.GetEmployees(organizationId);
            }catch(Exception ex)
            {
                throw new Exception("Error occured while fetching empoyee datails" + ex.Message);
            }
        }

        public async Task<EmployeeDTO> GetEmployeeDetail(int emploeeId)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeDetails(emploeeId);
                //if (employee == null)
                //{
                //    throw new Exception("Employee details not found");
                //}
                return employee;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while fetching employee details" + ex.Message);
            }
        }
        public async Task<IEnumerable<Employee>> GetEmployeeCreatedByUser(string userId, int organizationId)
        {
            try
            {
                if(await _userManager.FindByIdAsync(userId) == null) 
                    throw new Exception($"User with id {userId} can not be found");

                bool isOrganizationExisits = await _organizationRepository.IsOrganizationExisits(organizationId);
                if (!isOrganizationExisits)
                    throw new Exception($"Organization with id {organizationId} doesn't exists");

                IEnumerable<Employee> employees = await _employeeRepository.GetEmployeesCreatedByUser(userId, organizationId);
                //if (employees == null)
                //{
                //    throw new Exception("Employee details not found");
                //}
                return employees;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while fetching employee details" + ex.Message);
            }
        }

        public async Task<IEnumerable<EmployeeDTO>> GetHrList(int organizationId, string userId)
        {
            try
            {
                if (await _userManager.FindByIdAsync(userId) == null) throw new Exception("User not found");

                IEnumerable<EmployeeDTO> hrList = await _employeeRepository.GetAllHr(organizationId);
                return hrList;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<IEnumerable<EmployeeDTO>> GetEmployeeInUserRole(int organizationId, string userId)
        {
            try
            { 
                if (await _userManager.FindByIdAsync(userId) == null) throw new Exception("User not found");

                IEnumerable<EmployeeDTO> empList = await _employeeRepository.GetEmployeesInuserRole(organizationId);
                return empList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
