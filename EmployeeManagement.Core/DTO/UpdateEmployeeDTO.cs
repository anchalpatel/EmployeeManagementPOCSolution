using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Core.DTO
{
    public class UpdateEmployeeDTO
    {
        public string FirstName { get; set; } = null;

        public string LastName { get; set; } = null;

        public string Email { get; set; } = null;

        public string Address { get; set; } = null;

        public string PhoneNumber { get; set; } = null;
        public string? Roles { get; set; }
    }
}
