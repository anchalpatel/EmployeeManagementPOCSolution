


using EmployeeManagement.Application.DTO;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserDTO> RegisterUserAsync(RegisterUser registerDto);
        Task<UserDTO> AuthenticateUserAsync(LoginModel loginDto);
        Task<bool> LogoutAsync();
        Task<bool> RemoveUserAsync(string userId);
        Task<bool> UpdateEmail(string userId, string email);
    }
}
