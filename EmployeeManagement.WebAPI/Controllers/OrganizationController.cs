using System.Security.Claims;
using EmployeeManagement.Application.ServiceInterface;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateTokenPayloadActionFilter]
    public class OrganizationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrganizationService _organizationService;

        public OrganizationController(UserManager<ApplicationUser> userManager, IOrganizationService organizationService)
        {
            _userManager = userManager;
            _organizationService = organizationService;
        }
        [HttpPost("CreateOrganization")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateOrganization([FromBody]OrganizationDTO model)
        {
            if (!ModelState.IsValid) throw new Exception("Please enter valid data");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized(new ResponseDTO<object> { Success = false, Message = "User id not found in token" });
            try
            {
                var organization = await _organizationService.CreateOrganization(model, userId);
                return Ok(new ResponseDTO<object>{ Success = true, Message = "Organization created successfully", Data = organization });
            }catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>{ Message = "An unexpected error occurred", Error = ex.Message });
            }
        }

        [HttpPut("UpdateOrganization/{organizationId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationDTO model, int organizationId)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDTO<object> { Success = false, Message = "Invalid parameters" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized(new ResponseDTO<object> { Success = false, Message = "User id not found in token" });

            try
            {
                var organization = await _organizationService.UpdateOrganization(model, organizationId, userId);
                return Ok(new ResponseDTO<object>{ Success = true, Message = "Organization created successfully", Data = organization });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseDTO<object> { Success = false, Message = "UnAuthorized", Error = ex.ToString() });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseDTO<object> { Success = false, Error = ex.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("addAdmin/{organizationId}")]
        public async Task<IActionResult> AddAdmin([FromBody]CreateEmployeeDTO model, int organizationId)
        {
            try
            {
                if (!ModelState.IsValid) throw new Exception("Please enter valid data");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized("user id not found in token");
                var admin = await _organizationService.AddAdmin(model, userId, organizationId);
                return Ok(new { success = true, Data =  admin, Message = "Admin Created successfully" });
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("RemoveAdmin/{employeeId}")]
        public async Task<IActionResult> RemoveAdmin(int employeeId)
        {
            try
            {
              
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized("user id not found in token");

                var superAdmin = await _organizationService.RemoveAdmin(employeeId, userId);
                return Ok(new { success = true, Data = superAdmin, Message = "Admin Removed successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }
        [HttpGet("GetOrganizationDetails/{organizationId}")]
        [Authorize]
        public async Task<IActionResult> GetOrganizationDetails(int organizationId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized("user id not found in token");
            
                var organization = _organizationService.GetOrganizationAsync(organizationId);
                return Ok(new { success = true, Message = "Organization data fetched successfully", Data = organization});
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error rrcured", error = ex.Message });
            }
        }
        [HttpGet("GetAllOrganizations")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllOrganizationData()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized("user id not found in token");

                var organization = _organizationService.GetAllOrganizationData();
                return Ok(new { success = true, Message = "Organization data fetched successfully", Data = organization });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error rrcured", error = ex.Message });
            }
        }

        [HttpDelete("DeleteOrganization/{organizationId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> DeleteOrganization(int organizationId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized("user id not found in token");

                bool IsDeleted = await _organizationService.DeleteOrganization(organizationId, userId);
                if (IsDeleted) return Ok(new { Success = true, Message = "Organization deleted successfully" });

                throw new Exception("Error occured while deleting employee");
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error orrcured", error = ex.Message });
            }
        }
        [HttpGet("GetAdmin/{organizationId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> GetAdmin(int organizationId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized("User id not found in token");

                var admin = await _organizationService.GetAdmin(organizationId);
                return Ok(new { Success = true, Message = "Admin fetched successfully", Data = admin });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error ororcured", error = ex.Message });
            }
        }
    }
}
