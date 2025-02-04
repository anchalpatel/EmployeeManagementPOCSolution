
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.WebAPI;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Application.ServiceInterface;
namespace EmployeeManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateTokenPayloadActionFilter]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IEmployeeService _employeeService;

        public AdminController(IAdminService adminService, IEmployeeService employeeService)
        {
            _adminService = adminService;
            _employeeService = employeeService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateHr")]
        [ValidateTokenPayloadActionFilter]
        public async Task<IActionResult> CreateHr(CreateEmployeeDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Please enter valid data");
                }
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var organizationId = User.FindFirst(c => c.Type == "OrganizationId")?.Value;

                if (userId == null || organizationId == null)
                {
                    return Unauthorized(new ResponseDTO<object> { Success = false, Message = "User id or organization id not found in token" });

                }
                var hr = await _adminService.CreateHr(model, userId, Convert.ToInt32(organizationId));
                return Ok(new ResponseDTO<object>{ Success = true, Data = hr, Message = "Hr Created successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseDTO<object> { Success = false, Message="Anauthorized", Error = ex.ToString()});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseDTO<object> { Success = false, Message = "Invalid Parameters", Error = ex.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>{ Success = false, Message = "An unexpected error occurred", Error = ex.Message });
            }
        }
        [HttpDelete("RemoveHr/{employeeId}")]
        [Authorize(Roles = "Admin")]
        [ValidateTokenPayloadActionFilter]
        public async Task<IActionResult> DeleteHr(int employeeId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Please enter valid data");
                }
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new ResponseDTO<object> { Success = false, Message = "User id not found in token" });

                }
                bool isDeleted = await _adminService.RemoveHr(employeeId, userId);
                return Ok(new ResponseDTO<object>{ Success = true, Message = "Hr Deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseDTO<object> { Success = false, Message = "Unauthorized", Error = ex.ToString() });

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseDTO<object> { Success = false, Message = "User Id or Organization Id not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>{ Success = false, Message = "An unexpected error occurred", Error = ex.Message });
            }
        }

        [HttpGet("GetAllHR/{organizationId}")]
        [Authorize(Roles = "Admin")]
        [ValidateTokenPayloadActionFilter]
        public async Task<IActionResult> GetHR(int organizationId)
        {
            try
            {
            
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new ResponseDTO<object> { Success = false, Message = "User id not found in token" });
                }
                var hrList = await _employeeService.GetHrList(organizationId, userId);
                return Ok(new ResponseDTO<object>{ Success = true, Message = "Hr Deleted successfully" , Data = hrList});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseDTO<object> { Success = false, Message = "Unauthorized", Error = ex.ToString() });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseDTO<object> { Success = false, Message = "User Id or Organization Id not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>{ Success = false, Message = "An unexpected error occurred", Error = ex.ToString() });
            }
        }
    }
}
