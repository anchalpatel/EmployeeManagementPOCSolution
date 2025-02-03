using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RoleRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<bool> AddRole(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user == null)
                {
                    throw new ArgumentException("User not found");
                }
                var role = await _roleManager.FindByNameAsync(roleName);
                if(role == null)
                {
                    throw new Exception("Role not found");
                }
                var userRole = await _context.Set<ApplicationUserRoles>().FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id && ur.IsDeleted == true);
                if(userRole != null)
                {
                    userRole.IsDeleted = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return true;
                }
                throw new Exception("User can not be added to the role" + result.Errors);
            }catch (Exception ex)
            {
                throw new Exception( "Excdption occured while adding user to the role       "+ ex);
            }
        }

        public async Task<bool> IsInRole(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                if (user.IsDeleted)
                {
                    return false;
                }
                var roles = await _roleManager.FindByNameAsync(roleName);
                if (roles == null)
                {
                    throw new Exception($"Role {roleName} not found");
                }
                var userRole = await _context.Set<ApplicationUserRoles>().FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roles.Id);
                if (userRole != null && userRole.IsDeleted == false)
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while checking user roles " + ex);
            }
        }

        public async Task<bool> RemoveAllRoles(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);

                foreach (var roleName in roles)
                {
                    
                    var role = await _context.Roles
                                              .FirstOrDefaultAsync(r => r.Name == roleName);

                    if (role != null)
                    {
                        
                        var userRole = await _context.UserRoles
                                                      .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);

                        if (userRole != null)
                        {
                            userRole.IsDeleted = true;
                        }
                        else
                        {
                            throw new Exception($"User-role association for role '{roleName}' not found.");
                        }
                    }
                    else
                    {
                        throw new Exception($"Role '{roleName}' not found.");
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while removing roles: " + ex);
            }
        }


        public async Task<bool> RemoveRole(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }
                //var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                var role = await _roleManager.FindByNameAsync(roleName);
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(roleUser => roleUser.UserId == userId && roleUser.RoleId == role.Id);
                if (userRole != null)
                {
                    userRole.IsDeleted = true;
                    return true;
                }
                else
                {
                    throw new Exception($"User {user.UserName} can not be deleted from role {roleName}");
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured in adding role : " + ex);
            }
        }

        public async Task<bool> UpdateRole(string userId, string newRole)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }

                var oldRoles = await _context.UserRoles
                                              .Where(ur => ur.UserId == userId)
                                              .ToListAsync();

                var newRoleEntity = await _roleManager.FindByNameAsync(newRole);
                if (newRoleEntity == null)
                {
                    throw new Exception($"Role '{newRole}' not found.");
                }

                bool updated = false;

                foreach (var oldRole in oldRoles)
                {
                    var oldRoleEntity = await _roleManager.FindByIdAsync(oldRole.RoleId);
                    if (oldRoleEntity == null)
                    {
                        throw new Exception($"Role with ID {oldRole.RoleId} not found.");
                    }
                    if (oldRoleEntity.Name == newRole && oldRole.IsDeleted)
                    {
                        oldRole.IsDeleted = false;
                        updated = true;
                    }
                    else
                    {
                        oldRole.IsDeleted = true;
                    }
                }

                if (!oldRoles.Any(ur => ur.RoleId == newRoleEntity.Id && !ur.IsDeleted))
                {
                    var addResult = await _userManager.AddToRoleAsync(user, newRole);
                    if (!addResult.Succeeded)
                    {
                        throw new Exception("Failed to add user to the new role.");
                    }
                    updated = true;
                }

                if (updated)
                {
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while updating role: " + ex.Message, ex);
            }
        }



    }
}
