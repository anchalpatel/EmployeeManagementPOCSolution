using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.ServiceInterface;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<string> GenerateToken(string userId, string userName, int organiationId, string createdBy, string organizationName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await (from duser in _dbContext.Users
                               join ur in _dbContext.UserRoles
                               on duser.Id equals ur.UserId
                               join uRole in _dbContext.Roles
                               on ur.RoleId equals uRole.Id
                               where duser.Id == userId
                               && ur.IsDeleted == false
                               && duser.IsDeleted == false
                               && uRole.IsDeleted == false
                               select uRole.Name).ToListAsync();
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("Name", userName),
                new Claim("OrganizationId", organiationId.ToString()),
                new Claim("CreatedBy", createdBy),
                new Claim("OrganizationName", organizationName)
            };
           
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            _logger.Log(LogLevel.Warning, "TOKEN" + creds);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
