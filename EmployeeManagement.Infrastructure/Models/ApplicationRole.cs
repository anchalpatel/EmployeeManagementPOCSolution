using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Models
{
    public class ApplicationRole : IdentityRole
    {

        public bool IsDeleted { get; set; } = false;
    }
}
