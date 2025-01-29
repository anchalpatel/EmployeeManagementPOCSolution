using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Infrastructure.Services;

namespace EmployeeManagement.WebAPI
{
    public static class ServiceRegistrationExtension
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<AuthService>();

            //Organization
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<OrganizationService>();

            //Employee
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<EmployeeService>();

            //Admin
            services.AddScoped<AdminService>();

            //Role
            services.AddScoped<IRoleRepository, RoleRepository>();

            //Account
            //services.AddScoped<IAccountRepository, AccountRepository>();
            //services.AddScoped<AccountService>();

            //Email Service
            //services.AddScoped<EmailService>();

        }
    }

}
