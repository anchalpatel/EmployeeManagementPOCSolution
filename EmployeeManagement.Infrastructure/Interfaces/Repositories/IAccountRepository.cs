using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> SendPasswordResetLink(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string password);
    }
}
