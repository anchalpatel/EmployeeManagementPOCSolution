using System.IdentityModel.Tokens.Jwt;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.WebAPI
{
    //[AttributeUsage(AttributeTargets.Method)]
    public class ValidateTokenPayloadActionFilter : ActionFilterAttribute
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var _dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", "").Trim();
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new BadRequestObjectResult(new { success = false, Message = "Token not found" });
                return;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null)
                {
                    context.Result = new BadRequestObjectResult(new { success = false, Message = "Invalid JWT Token" });
                    return;
                }

                var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);

                if (claims.TryGetValue("userId", out var userId))
                {
                    var user = await _dbContext.Set<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
                    if (user == null)
                    {
                        context.Result = new BadRequestObjectResult(new { success = false, Message = "User not found" });
                        return;
                    }
                }
                else
                {
                    context.Result = new BadRequestObjectResult(new { success = false, Message = "User Id not found" });
                    return;
                }

                var roles = claims.GetValueOrDefault("Role", "").Split(',');

                if (roles.Contains("SuperAdmin"))
                {
                    if (claims.TryGetValue("OrganizationId", out string organizationId) && organizationId != "0")
                    {
                        context.Result = new BadRequestObjectResult(new { success = false, Message = "For SuperAdmin, Organization Id must be 0" });
                        return;
                    }

                    if (claims.TryGetValue("CreatedBy", out string createdBy) && createdBy != "")
                    {
                        context.Result = new BadRequestObjectResult(new { success = false, Message = "For SuperAdmin, CreatedBy must be empty string" });
                        return;
                    }
                }
                else
                {
                    if (claims.TryGetValue("OrganizationId", out string organizationId))
                    {
                        var organizationIdInt = Convert.ToInt32(organizationId);
                        var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == organizationIdInt);
                        if (organization == null)
                        {
                            context.Result = new BadRequestObjectResult(new { success = false, Message = "Organization not found" });
                            return;
                        }

                        if (claims.TryGetValue("OrganizationName", out string organizationName) && !string.Equals(organizationName, organization.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Result = new BadRequestObjectResult(new { success = false, Message = "Organization Name mismatch" });
                            return;
                        }
                    }
                }

                await next();
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new { success = false, Message = $"Error while validating token: {ex.Message}" });
                return;
            }
        }
    }
}
