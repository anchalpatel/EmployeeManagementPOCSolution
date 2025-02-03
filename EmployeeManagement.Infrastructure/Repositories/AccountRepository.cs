using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public Task<bool> ResetPasswordAsync(string email, string token, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendPasswordResetLink(string email)
        {
            throw new NotImplementedException();
        }
    }
}
