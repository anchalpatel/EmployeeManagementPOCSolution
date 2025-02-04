
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;

using EmployeeManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRoleRepository _roleRepository;
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IRoleRepository roleRepository, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleRepository = roleRepository;
            _dbContext = dbContext;
        }
        public async Task<UserDTO> AuthenticateUserAsync(LoginModel loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null && user.IsDeleted == false)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return new UserDTO
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email
                        };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while authenticating user " + ex.Message);
            }
        }

        
        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while logginh out the user " + ex.Message);
            }
        }

        public async Task<UserDTO> RegisterUserAsync(RegisterUser registerDto)
        {
            try
            {
                //var oldUser = await _userManager.FindByEmailAsync(registerDto.Email);
                //if (oldUser != null)
                //{
                //    if (oldUser.IsDeleted == true)
                //    {
                //        oldUser.IsDeleted = false;
                //        oldUser.PasswordHash = registerDto.Password;
                //        oldUser.UserName = registerDto.Email;
                //        await _dbContext.SaveChangesAsync();
                //    }
                //    return new UserDTO
                //    {
                //        UserId = oldUser.Id,
                //        UserName = oldUser.Email,
                //        Email = oldUser.Email,
                //    };
                //}

                var user = new ApplicationUser()
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                };
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                {
                    return new UserDTO
                    {
                        UserId = user.Id,
                        UserName = user.Email,
                        Email = user.Email,
                    };
                }
                else
                {
                    throw new Exception("User can not be created");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while registering the user " + ex.Message);
            }
        }

        public async Task<bool> RemoveUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                var removeRolesResults = await _roleRepository.RemoveAllRoles(userId);
                if (removeRolesResults)
                {
                    user.IsDeleted = true;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new Exception($"User cannot be removed from roles");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while deleting user " + ex.Message);
            }
        }

        public async Task<bool> UpdateEmail(string userId, string email)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                if (await _userManager.FindByEmailAsync(email) == null)
                {
                    user.Email = email;
                    user.UserName = email;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("Failed to update the user's email");
                    }
                }
                else
                {
                    throw new Exception($"User already exist with email {email}");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while updating user");
            }
        }
    }
}
