using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DefaultValue(0)]
        public bool IsDeleted { get; set; }
    }
}
