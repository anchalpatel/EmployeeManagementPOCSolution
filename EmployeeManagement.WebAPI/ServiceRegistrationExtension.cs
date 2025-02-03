using EmployeeManagement.Application.ServiceInterface;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Repositories;

namespace EmployeeManagement.WebAPI
{
    public static class ServiceRegistrationExtension
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService ,AuthService>();

            //Organization
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IOrganizationService ,OrganizationService>();

            //Employee
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService ,EmployeeService>();

            //Admin
            services.AddScoped<IAdminService ,AdminService>();

            //Role
            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IJwtService ,JwtService>();

            //Account
            //services.AddScoped<IAccountRepository, AccountRepository>();
            //services.AddScoped<AccountService>();

            //Email Service
            //services.AddScoped<EmailService>();

        }
    }

}
