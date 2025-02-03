using EmployeeManagement.Application.ServiceInterface;
using EmployeeManagement.Core.DTO;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;

namespace EmployeeManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public AuthService(IUserRepository userRepository, IJwtService jwtService,
                            IEmployeeRepository employeeRepository,
                            IOrganizationRepository organizationRepository) {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _employeeRepository = employeeRepository;
            _organizationRepository = organizationRepository;
        }
        public async Task<string> RegisterAsync(RegisterUser model, int organizationId = 0, string createdBy = null)
        {
            var user = await _userRepository.RegisterUserAsync(model);
            if(user != null)
            {
                OrganizationDTO organization = new OrganizationDTO();
                string organizationName = "";
                if (organizationId != 0)
                {
                    organization = await _organizationRepository.GetOrganizationsDetials(organizationId);
                    organizationName = organization.Name;
                    
                }
                
                return await _jwtService.GenerateToken(user.UserId, user.UserName, organizationId, createdBy, organizationName);
            }
            return null;
        }

        public async Task<string> LoginAsync(LoginModel model)
        {
            var user = await _userRepository.AuthenticateUserAsync(model);
            if(user != null)
            {
                var employee = await _employeeRepository.GetEmployeeByUserId(user.UserId);
                int organizationId = 0;
                OrganizationDTO organization = new OrganizationDTO();
                string organizationName = "";
                string createdBy = "";
                if (employee != null)
                {
                    organizationId = employee.OrganizationId;
                    createdBy = employee.CreatedBy;
                   
                    if (organizationId != 0)
                    {
                        organization = await _organizationRepository.GetOrganizationsDetials(organizationId);
                        if (organization == null)
                        {
                            
                        }
                        else
                        {
                            organizationName = organization.Name;
                        }
                    }
                }
                
                return await _jwtService.GenerateToken(user.UserId, user.UserName, organizationId, createdBy, organizationName);
            }
            return null;
        }

        public async Task<bool> Logout()
        {
            var isLoggedIn = await _userRepository.LogoutAsync();
            return isLoggedIn;
        }
    }
}
