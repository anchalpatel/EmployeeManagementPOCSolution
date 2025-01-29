using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTO
{
    public class EmployeeDTO
    {
        public int Id {  get; set; }
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
        public int OrganizationId { get; set; }
        
        public string? userId { get; set; }
       
        public string? CreatedBy { get; set; }

        public DateTime CreareAt { get; set; }

        public DateTime UpdateAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Password { get; set; }
       
        public List<string>? Roles { get; set; } = new List<string>();
    }
}
