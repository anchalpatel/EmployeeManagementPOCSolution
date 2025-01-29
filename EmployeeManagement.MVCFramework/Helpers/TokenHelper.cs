using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace EmployeeManagement.MVCFramework.Helpers
{
    public static class TokenHelper
    {
        public static string GetClaimFromToken(string token, string claimType)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jsonToken != null)
                {
                    var claim = jsonToken.Claims.FirstOrDefault(c => c.Type == claimType);
                    return claim?.Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading token: " + ex.Message);
                return null;
            }
        }
        public static List<string> GetRolesFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roles = jwtToken?.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();
            Debug.WriteLine("TOKEN INFORMATION : " + jwtToken + roles);
            return roles ?? new List<string>(); 
        }
    }
}