using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace EmployeeManagement.MVCFramework.CustomAttributes
{
    public class TokenValidationAttribute : ValidationAttribute
    {
        
        public override bool IsValid(object value)
        {
            var token = HttpContext.Current.Session["token"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                var expiry = jsonToken?.Claims.FirstOrDefault(c => c.Type=="exp")?.Value;
                if(expiry == null)
                {
                    ErrorMessage = "Token Expiration Details Not Found";
                    return false;
                }
                else
                {
                    var expiryTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry)).DateTime;
                    if(expiryTime < DateTime.UtcNow)
                    {
                        ErrorMessage = "TOken has expired";
                        return false;
                    }
                }
                var roles = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value.Split(',');
                if(roles == null)
                {
                    ErrorMessage = "Access Denied";
                    return false; ;
                }

                var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if(userId == null)
                {
                    ErrorMessage = "User id not found";
                    return false;
                }

                var name = jsonToken?.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
                if (name == null)
                {
                    ErrorMessage = "Name id not found";
                    return false;
                }
                var organizationId = jsonToken?.Claims.FirstOrDefault(c => c.Type == "OrganizationId")?.Value;
                if (organizationId == null)
                {
                    ErrorMessage = "Name id not found";
                    return false;
                }
                var createdBy = jsonToken?.Claims.FirstOrDefault(c => c.Type == "OrganizationId")?.Value;
                if (createdBy == null)
                {
                    ErrorMessage = "User data not found";
                    return false;
                }
                var organizationName = jsonToken?.Claims.FirstOrDefault(c => c.Type == "OrganizationName")?.Value;
                if (organizationName == null)
                {
                    ErrorMessage = "Name id not found";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}