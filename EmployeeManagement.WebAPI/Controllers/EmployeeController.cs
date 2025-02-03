using System.Security.Claims;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Core.Entites;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.ServiceInterface;

namespace EmployeeManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateTokenPayloadActionFilter]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpPost("CreateEmployee")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDTO model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string organizationIdStr = User.FindFirst(c => c.Type == "OrganizationId")?.Value;
                int organizationId = Convert.ToInt32(organizationIdStr);
                if (userId == null || organizationId == null)
                {
                    return BadRequest("User Id or Organization Id not found");
                }

                var employee = await _employeeService.CreateEmployee(model, userId, organizationId);
                if (employee == null)
                {
                    throw new Exception("Employee can not be created");
                }
                return Ok(new { Success = true, Mesaage = "Employee Created successfully", Data = employee });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error occured while creating employee", Error = ex.Message });
            }
        }
        [HttpPut("UpdateEmployee/{employeeId}")]
        [Authorize(Roles = "SuperAdmin, Admin, HR")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDTO model, int employeeId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string organizationIdStr = User.FindFirst(c => c.Type == "OrganizationId")?.Value;
                int organizationId = Convert.ToInt32(organizationIdStr);
                if (userId == null || organizationId == null)
                {
                    return BadRequest("User Id or Organization Id not found");
                }

                var employee = await _employeeService.UpdateEmployee(model, employeeId, userId);
                if (employee == null)
                {
                    throw new Exception("Employee can not be updated");
                }
                return Ok(new { Success = true, Mesaage = "Employee Updated successfully", Data = employee });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error occured while updating employee", Error = ex.Message });
            }
        }
        [HttpDelete("DeleteEmployee/{employeeId}")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string organizationIdStr = User.FindFirst(c => c.Type == "OrganizationId")?.Value;
                int organizationId = Convert.ToInt32(organizationIdStr);
                if (userId == null || organizationId == null)
                {
                    return BadRequest("User Id or Organization Id not found");
                }

                var IsDeleted = await _employeeService.RemoveEmpoyee(employeeId, userId);
                if (!IsDeleted)
                {
                    throw new Exception("Employee can not be deleted");
                }
                return Ok(new { Success = true, Mesaage = "Employee Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error occured while deleting employee", Error = ex.Message });
            }
        }
        [HttpGet("GetAllEmployee/{organizationId}")]
        [Authorize(Roles = "Admin, User, HR")]
        public async Task<IActionResult> GetAllEmployees(int organizationId)
        {
            try
            {
                IEnumerable<EmployeeDTO> employees = await _employeeService.GetAllEmployees(organizationId);
                if (employees == null)
                {
                    return NotFound(new { Success = false, Message = "No employee found" });
                }
                return Ok(new { Success = true, Data = employees, Message = "Employees fetched successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Employee list can not be fetched", Error = ex.Message });
            }
        }

        [HttpGet("GetEmployeeDetails/{employeeId}")]
        [Authorize(Roles = "SuperAdmin, Admin, HR, User")]
        public async Task<IActionResult> GetEmployeeDetails(int employeeId)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeDetail(employeeId);
                return Ok(new { Success = true, Message = "Employee details fetched successfully", Data = employee });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Employee can not be fetched" + ex.Message, Error = ex.Message });
            }
        }

        [HttpGet("GetEmployeesCreatedByUser")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> GetEmployeesCreatedByUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string organizationIdStr = User.FindFirst(c => c.Type == "OrganizationId")?.Value;
                int organizationId = Convert.ToInt32(organizationIdStr);
                if (userId == null || organizationId == null)
                {
                    return BadRequest("User Id or Organization Id not found");
                }
                var employees = await _employeeService.GetEmployeeCreatedByUser(userId, organizationId);
                return Ok(new { Success = true, Message = "Employee details fetched successfully", Data = employees });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Employee can not be fetched", Error = ex.Message });
            }
        }
        [HttpGet("GetAllHr")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllHr()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string organizationIdStr = User.FindFirst(c => c.Type == "OrganizationId")?.Value;
                int organizationId = Convert.ToInt32(organizationIdStr);
                if (userId == null || organizationId == null)
                {
                    return BadRequest("User Id or Organization Id not found");
                }
                var hrList = await _employeeService.GetHrList(organizationId, userId);
                return Ok(new { Success = true, Message = "Hr details fetched successfully", Data = hrList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Employee can not be fetched", Error = ex.Message });
            }
        }
        [HttpGet("GetEmployeeInUserRole")]
        [Authorize(Roles = "HR, Admin")]
        public async Task<IActionResult> GetEmployeeInUserRole()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string organizationIdStr = User.FindFirst(c => c.Type == "OrganizationId")?.Value;
                int organizationId = Convert.ToInt32(organizationIdStr);
                if (userId == null || organizationId == null)
                {
                    return BadRequest("User Id or Organization Id not found");
                }
                var empList = await _employeeService.GetEmployeeInUserRole(organizationId, userId);
                return Ok(new { Success = true, Message = "Hr details fetched successfully", Data = empList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Employee can not be fetched", Error = ex.Message });
            }
        }
    }
}
