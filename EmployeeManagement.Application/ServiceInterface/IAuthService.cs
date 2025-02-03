using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Core.DTO;


namespace EmployeeManagement.Application.ServiceInterface
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterUser model, int organizationId = 0, string createdBy = null);
        Task<string> LoginAsync(LoginModel model);
        Task<bool> Logout();

    }
}
